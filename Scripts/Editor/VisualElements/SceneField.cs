using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    internal sealed class SceneField : AdvancedPopupField<int>
    {
        internal SceneField(string label) : base(label)
        {
            formatListItemCallback = FormatListItem;
            formatSelectedValueCallback = FormatSelectedValue;
            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        internal event Action ChoicedUpdated;

        private protected override void OnCreateMenu() => UpdateChoices();

        private protected override void OnBeforeMenuShow(AbstractGenericMenu menu)
        {
            menu.AddSeparator(string.Empty);
            menu.AddItem("Open Build Profiles...", false, OpenBuildProfiles);
        }

        private void UpdateChoices()
        {
            List<int> choices = new();
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i += 1)
            {
                choices.Add(i);
            }
            this.choices = choices;
            ChoicedUpdated?.Invoke();
        }

        #region EventHandlers
        private void OnAttachToPanel(AttachToPanelEvent context) => EditorBuildSettings.sceneListChanged += OnSceneListChanged;

        private void OnDetachFromPanel(DetachFromPanelEvent context) => EditorBuildSettings.sceneListChanged -= OnSceneListChanged;

        private void OnSceneListChanged() => UpdateChoices();
        #endregion

        private static string FormatSelectedValue(int value)
        {
            if ((value >= EditorBuildSettings.scenes.Length) || (value < 0))
            {
                return string.Empty;
            }
            EditorBuildSettingsScene scene = EditorBuildSettings.scenes[value];
            return Path.GetFileNameWithoutExtension(scene.path);
        }

        private static string FormatListItem(int value)
        {
            EditorBuildSettingsScene scene = EditorBuildSettings.scenes[value];
            string sceneName = Path.GetFileNameWithoutExtension(scene.path);
            return $"{value}: {sceneName}";
        }

        private static void OpenBuildProfiles()
        {
            BuildPlayerWindow window = EditorWindow.GetWindow<BuildPlayerWindow>();
            window.Show();
        }
    }
}
