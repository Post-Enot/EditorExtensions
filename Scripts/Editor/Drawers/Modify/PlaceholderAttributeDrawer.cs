using PostEnot.Toolkits;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(PlaceholderAttribute))]
    internal sealed class PlaceholderAttributeDrawer : ModifyAttributeDrawer<PlaceholderAttribute>
    {
        private protected override void OnAttach(
            SerializedProperty property,
            PropertyField propertyField,
            FieldInfo fieldInfo,
            PlaceholderAttribute attribute) => Modify(propertyField, attribute);

        private protected override void AfterAttach(
            SerializedProperty property,
            PropertyField propertyField,
            FieldInfo fieldInfo,
            PlaceholderAttribute attribute) => Modify(propertyField, attribute);

        private void Modify(PropertyField propertyField, PlaceholderAttribute attribute)
        {
            if (attribute == null)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(attribute.Text))
            {
                return;
            }
            if (propertyField == null)
            {
                return;
            }
            TextField textField = propertyField.Q<TextField>();
            if (textField == null)
            {
                return;
            }
            textField.textEdition.placeholder = attribute.Text;
        }
    }
}
