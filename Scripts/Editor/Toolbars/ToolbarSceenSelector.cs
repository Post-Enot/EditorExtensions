using System;
using System.Collections.Generic;
using System.IO;
using Unity.Scripting.LifecycleManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.SceneManagement;
using static PostEnot.EditorExtensions.Editor.EditorAttributesSettingsAsset;

namespace PostEnot.EditorExtensions.Editor
{
    public sealed class ToolbarSceenSelector
    {
        static ToolbarSceenSelector()
        {
            RefreshSceneList();
            EditorApplication.projectChanged += RefreshSceneList;
            SceneManager.activeSceneChanged += SceneSwitched;
            EditorSceneManager.activeSceneChangedInEditMode += SceneSwitched;
        }

        private const string _elementPath = "PostEnot/Scene Selector";

        [NoAutoStaticsCleanup] private static string[] _scenePaths;

        [MainToolbarElement(_elementPath, defaultDockPosition = MainToolbarDockPosition.Left)]
        public static MainToolbarElement CreateSceneSelectorDropdown()
        {
            string activeSceneName;
            if (Application.isPlaying)
            {
                Scene scene = SceneManager.GetActiveScene();
                activeSceneName = scene.name;
            }
            else
            {
                Scene scene = EditorSceneManager.GetActiveScene();
                activeSceneName = scene.name;
            }
            if (activeSceneName.Length == 0)
            {
                activeSceneName = "Untitled";
            }
            Texture2D icon = EditorGUIUtility.IconContent("UnityLogo").image as Texture2D;
            MainToolbarContent content = new(activeSceneName, icon, "Select active scene");
            return new MainToolbarDropdown(content, ShowDropdownMenu);
        }

        private static void ShowDropdownMenu(Rect dropDownRect)
        {
            GenericMenu menu = new();

            if (_scenePaths.Length == 0)
            {
                menu.AddDisabledItem(new GUIContent("No Scenes in Project"));
                menu.DropDown(dropDownRect);
                return;
            }

            IReadOnlyList<ScenePathReplacement> replacements = EditorAttributesSettingsAsset.GetSettings().ScenePathReplacements;

            // Считаем display-пути один раз и сортируем по ним.
            // GenericMenu трактует '/' в label как разделитель подменю,
            // поэтому путь "Scenes/Level1/Foo" автоматически превращается
            // в иерархию Scenes → Level1 → Foo. Замены префиксов позволяют
            // переопределить эту иерархию как угодно.
            var entries = new List<(string DisplayPath, string ScenePath)>(_scenePaths.Length);
            foreach (string scenePath in _scenePaths)
            {
                entries.Add((GetDisplayPath(scenePath, replacements), scenePath));
            }
            entries.Sort((a, b) =>
                string.Compare(a.DisplayPath, b.DisplayPath, StringComparison.OrdinalIgnoreCase));

            foreach ((string displayPath, string scenePath) in entries)
            {
                string capturedScenePath = scenePath;
                menu.AddItem(new GUIContent(displayPath), false, () => SwitchScene(capturedScenePath));
            }

            menu.DropDown(dropDownRect);
        }

        private static string NormalizeSlashes(string path)
            => string.IsNullOrEmpty(path) ? path : path.Replace('\\', '/');

        private static string GetDisplayPath(string scenePath, IReadOnlyList<ScenePathReplacement> replacements)
        {
            // Работаем только с прямыми слэшами:
            //  - на Windows Directory.GetFiles вернёт 'Assets\Scenes\Foo.unity';
            //  - GenericMenu трактует только '/' как разделитель подменю.
            string displayPath = NormalizeSlashes(scenePath);

            if (replacements != null)
            {
                // Применяем ПЕРВУЮ подошедшую замену (порядок в списке = приоритет).
                foreach (ScenePathReplacement replacement in replacements)
                {
                    // Нормализуем и префикс — пользователь мог написать '\' вручную.
                    string prefix = NormalizeSlashes(replacement.Prefix);
                    if (string.IsNullOrEmpty(prefix))
                    {
                        continue;
                    }
                    if (displayPath.StartsWith(prefix, StringComparison.Ordinal))
                    {
                        string replacementValue = NormalizeSlashes(replacement.Replacement) ?? string.Empty;
                        displayPath = replacementValue + displayPath[prefix.Length..];
                        break;
                    }
                }
            }

            const string unityExtension = ".unity";
            if (displayPath.EndsWith(unityExtension, StringComparison.OrdinalIgnoreCase))
            {
                displayPath = displayPath[..^unityExtension.Length];
            }

            return displayPath;
        }

        private static void SwitchScene(string scenePath)
        {
            if (Application.isPlaying)
            {
                string sceneName = Path.GetFileNameWithoutExtension(scenePath);
                if (Application.CanStreamedLevelBeLoaded(sceneName))
                {
                    SceneManager.LoadScene(sceneName);
                }
                else
                {
                    Debug.LogError($"Scene '{sceneName}' is not in the Build Settings.");
                }
            }
            else
            {
                if (File.Exists(scenePath))
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        EditorSceneManager.OpenScene(scenePath);
                    }
                }
                else
                {
                    Debug.LogError($"Scene at path '{scenePath}' does not exist.");
                }
            }
        }

        private static void RefreshSceneList() => _scenePaths = Directory.GetFiles("Assets", "*.unity", SearchOption.AllDirectories);

        private static void SceneSwitched(Scene oldScene, Scene newScene) => MainToolbar.Refresh(_elementPath);
    }
}
