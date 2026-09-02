using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    internal class EditorAttributesSettingsProvider : SettingsProvider
    {
        internal EditorAttributesSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null)
            : base(path, scopes, keywords) => Settings = EditorAttributesSettingsAsset.GetSettings();

        public EditorAttributesSettingsAsset Settings { get; }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            base.OnActivate(searchContext, rootElement);
            SerializedObject serializedObject = new(Settings);
            SerializedProperty lineDecoratorProperty = serializedObject.FindProperty("lineDecoratorStyleSheet");
            PropertyField lineDecoratorField = new(lineDecoratorProperty, lineDecoratorProperty.displayName);
            lineDecoratorField.BindProperty(lineDecoratorProperty);
            SerializedProperty hierarchySpecialNamesProperty = serializedObject.FindProperty("hierarchySpecialNames");
            PropertyField hierarchySpecialNamesField = new(hierarchySpecialNamesProperty);
            hierarchySpecialNamesField.BindProperty(hierarchySpecialNamesProperty);
            rootElement.Add(lineDecoratorField);
            rootElement.Add(hierarchySpecialNamesField);
        }

        [SettingsProvider]
        internal static SettingsProvider InstantiateSettingsProvider()
            => new EditorAttributesSettingsProvider("Project/Editor Attributes", SettingsScope.Project);
    }
}
