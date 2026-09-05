using PostEnot.Toolkits;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(LabelAttribute), true)]
    internal sealed class LabelAttributeDrawer : ModifyAttributeDrawer<LabelAttribute>
    {
        private protected override void AfterAttach(
            SerializedProperty property,
            PropertyField propertyField,
            FieldInfo fieldInfo,
            LabelAttribute attribute)
        {
            if (attribute == null)
            {
                return;
            }
            if (propertyField == null)
            {
                return;
            }
            if (propertyField.label == string.Empty)
            {
                return;
            }
            propertyField.label = string.IsNullOrWhiteSpace(attribute.Label) ? string.Empty : attribute.Label;
        }
    }
}
