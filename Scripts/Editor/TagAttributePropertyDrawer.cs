using PostEnot.Toolkits;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(TagAttribute))]
    internal sealed class TagAttributePropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                return new Label($"Implement {nameof(TagAttribute)} to string field.");
            }
            TagField tagField = new(preferredLabel, property.stringValue);
            tagField.BindProperty(property);
            tagField.AddToClassList(BaseField<int>.alignedFieldUssClassName);
            return tagField;
        }
    }
}
