using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    internal class EditorAttributesSettingsProvider : SettingsProvider
    {
        internal EditorAttributesSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null)
            : base(path, scopes, keywords) { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            base.OnActivate(searchContext, rootElement);
            BuildUI(rootElement);
        }

        private void BuildUI(VisualElement rootElement)
        {
            rootElement.Clear();
            EditorAttributesSettingsAsset currentSettings = EditorAttributesSettingsAsset.GetSettings();
            // 1. Поле для выбора "актуального" ассета настроек
            ObjectField settingsAssetField = new("Settings Asset")
            {
                objectType = typeof(EditorAttributesSettingsAsset),
                allowSceneObjects = false,
                value = currentSettings
            };
            VisualElement inspectorContainer = new();
            settingsAssetField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue is EditorAttributesSettingsAsset newSettings)
                {
                    EditorAttributesSettingsAsset.SetSettings(newSettings);
                }
                // Пересобираем UI отложенно, т.к. текущий ObjectField будет удалён из rootElement
                rootElement.schedule.Execute(() => DrawInspector(inspectorContainer, settingsAssetField.value as EditorAttributesSettingsAsset));
            });
            rootElement.Add(settingsAssetField);
            rootElement.Add(inspectorContainer);
            DrawInspector(inspectorContainer, currentSettings);
        }

        private static void DrawInspector(VisualElement inspectorContainer, EditorAttributesSettingsAsset currentSettings)
        {
            inspectorContainer.Clear();
            if (currentSettings != null)
            {
                InspectorElement inspector = new(currentSettings);
                inspectorContainer.Add(inspector);

                // Сохраняем поведение: при изменении hierarchySpecialNames рассылаем событие
                SerializedObject serializedObject = new(currentSettings);
                SerializedProperty specialNamesProperty = serializedObject.FindProperty("hierarchySpecialNames");
                if (specialNamesProperty != null)
                {
                    inspector.TrackPropertyValue(
                        specialNamesProperty,
                        _ => currentSettings.InvokeSpecialNamesChanged());
                }
            }
        }

        [SettingsProvider]
        internal static SettingsProvider InstantiateSettingsProvider()
            => new EditorAttributesSettingsProvider("Project/Editor Attributes", SettingsScope.Project);
    }
}
