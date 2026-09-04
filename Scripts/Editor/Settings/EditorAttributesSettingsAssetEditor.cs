using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomEditor(typeof(EditorAttributesSettingsAsset))]
    internal sealed class EditorAttributesSettingsAssetEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();
            InspectorElement.FillDefaultInspector(container, serializedObject, this);
            PropertyField propertyField = container.Q<PropertyField>("PropertyField:hierarchySpecialNames");
            SerializedProperty serializedProperty = serializedObject.FindProperty("hierarchySpecialNames");

            propertyField.TrackPropertyValue(
                serializedProperty,
                _ =>
                {
                    EditorAttributesSettingsAsset settings = EditorAttributesSettingsAsset.GetSettings();
                    settings.InvokeSpecialNamesChanged();
                });
            return container;
        }
    }
}
