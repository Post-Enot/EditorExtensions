using PostEnot.Toolkits;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(LayerAttribute))]
    internal sealed class LayerAttributeDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            if (property.propertyType is not SerializedPropertyType.Integer)
            {
                return new Label($"Use Layer with int.");
            }
            LayerField layerField = new(preferredLabel, property.intValue);
            layerField.AddToClassList(BaseField<int>.alignedFieldUssClassName);
            layerField.BindProperty(property);
            return layerField;
        }
    }
}
