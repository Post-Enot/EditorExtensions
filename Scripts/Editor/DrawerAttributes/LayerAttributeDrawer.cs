using PostEnot.Toolkits;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(LayerAttribute))]
    internal sealed class LayerAttributeDrawer : BasePropertyDrawer<LayerAttribute>
    {
        private protected override VisualElement CreateProperty(
            SerializedProperty property,
            FieldInfo fieldInfo,
            LayerAttribute attribute)
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
