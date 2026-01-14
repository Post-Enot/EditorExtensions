using PostEnot.Toolkits;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(TagAttribute))]
    internal sealed class TagAttributeDrawer : BasePropertyDrawer<TagAttribute>
    {
        private protected override VisualElement CreateProperty(
            SerializedProperty property,
            FieldInfo fieldInfo,
            TagAttribute attribute)
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
