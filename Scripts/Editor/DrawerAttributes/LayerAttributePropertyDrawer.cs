using PostEnot.Toolkits;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(LayerAttribute))]
    internal sealed class LayerAttributePropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            if (property.propertyType is not SerializedPropertyType.Integer)
            {
                return new Label($"Use Layer with int.");
            }
            LayerField layerField = new(preferredLabel, property.intValue);
            layerField.BindProperty(property);
            layerField.AddToClassList(BaseField<int>.alignedFieldUssClassName);
            return layerField;
        }
    }
}
