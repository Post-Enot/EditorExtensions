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
            SerializedProperty serializedProperty = serializedObject.FindProperty("lineDecoratorStyleSheet");
            PropertyField propertyField = new(serializedProperty, serializedProperty.displayName);
            propertyField.BindProperty(serializedProperty);
            rootElement.Add(propertyField);
        }

        [SettingsProvider]
        internal static SettingsProvider InstantiateSettingsProvider()
            => new EditorAttributesSettingsProvider("Project/Editor Attributes", SettingsScope.Project);
    }
}
