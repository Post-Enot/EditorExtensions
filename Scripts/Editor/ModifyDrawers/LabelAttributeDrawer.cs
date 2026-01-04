using PostEnot.Toolkits;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(LabelAttribute))]
    internal sealed class LabelAttributeDrawer : ModifyAttributeDrawer<LabelAttribute>
    {
        // Необходим именно AfterAttach, т.к. при установке label = string.Empty происходит изменение иерархии,
        // а attribute.Label потенциально может быть null или string.Empty.
        private protected override void AfterAttach(
            SerializedProperty property,
            PropertyField propertyField,
            FieldInfo fieldInfo,
            LabelAttribute attribute)
        {
            if (propertyField != null)
            {
                propertyField.label = attribute.Label;
            }
        }
    }
}
