using PostEnot.Toolkits;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(LabelAttribute))]
    internal sealed class LabelAttributeDrawer : ModifyAttributeDrawer<LabelAttribute>
    {
        private protected override void AfterAttach(
            SerializedProperty property,
            PropertyField propertyField,
            FieldInfo fieldInfo,
            LabelAttribute attribute)
        {
            if ((propertyField != null) && (propertyField.label != string.Empty))
            {
                propertyField.label = attribute.Label;
            }
        }
    }
}
