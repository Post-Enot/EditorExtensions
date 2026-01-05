using PostEnot.Toolkits;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(TagAttribute))]
    internal sealed class TagAttributeDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            if (property.propertyType is not SerializedPropertyType.String)
            {
                return new Label($"Use Tag with string.");
            }
            TagField tagField = new(preferredLabel, property.stringValue);
            tagField.BindProperty(property);
            tagField.AddToClassList(BaseField<int>.alignedFieldUssClassName);
            return tagField;
        }
    }
}
