using System.IO;
using Unity.Scripting.LifecycleManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PostEnot.EditorExtensions.Editor
{
    public class ToolbarSceenSelector
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

        static void ShowDropdownMenu(Rect dropDownRect)
        {
            GenericMenu menu = new();
            if (_scenePaths.Length == 0)
            {
                menu.AddDisabledItem(new GUIContent("No Scenes in Project"));
            }
            foreach (string scenePath in _scenePaths)
            {
                string sceneName = Path.GetFileNameWithoutExtension(scenePath);
                menu.AddItem(new GUIContent(sceneName), false, () =>
                {
                    SwitchScene(scenePath);
                });
            }
            menu.DropDown(dropDownRect);
        }

        static void SwitchScene(string scenePath)
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

        static void RefreshSceneList() => _scenePaths = Directory.GetFiles("Assets", "*.unity", SearchOption.AllDirectories);

        static void SceneSwitched(Scene oldScene, Scene newScene) => MainToolbar.Refresh(_elementPath);
    }
}
