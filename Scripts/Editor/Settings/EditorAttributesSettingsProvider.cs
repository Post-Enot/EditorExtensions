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
            EditorAttributesSettingsAsset settings = EditorAttributesSettingsAsset.GetSettings();
            base.OnActivate(searchContext, rootElement);
            SerializedObject serializedObject = new(settings);
            SerializedProperty lineDecoratorProperty = serializedObject.FindProperty("lineDecoratorStyleSheet");
            PropertyField lineDecoratorField = new(lineDecoratorProperty, lineDecoratorProperty.displayName);
            lineDecoratorField.BindProperty(lineDecoratorProperty);
            SerializedProperty hierarchySpecialNamesProperty = serializedObject.FindProperty("hierarchySpecialNames");
            PropertyField hierarchySpecialNamesField = new(hierarchySpecialNamesProperty);
            hierarchySpecialNamesField.BindProperty(hierarchySpecialNamesProperty);
            hierarchySpecialNamesField.TrackPropertyValue(hierarchySpecialNamesProperty, SpecialNamesPropertyChanged);
            rootElement.Add(lineDecoratorField);
            rootElement.Add(hierarchySpecialNamesField);
        }

        private void SpecialNamesPropertyChanged(SerializedProperty serializedProperty)
        {
            EditorAttributesSettingsAsset settings = EditorAttributesSettingsAsset.GetSettings();
            settings.InvokeSpecialNamesChanged();
        }

        [SettingsProvider]
        internal static SettingsProvider InstantiateSettingsProvider()
            => new EditorAttributesSettingsProvider("Project/Editor Attributes", SettingsScope.Project);
    }
}
