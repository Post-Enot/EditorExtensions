using PostEnot.Toolkits;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
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
            PlaceholderAttribute attribute)
        {
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
