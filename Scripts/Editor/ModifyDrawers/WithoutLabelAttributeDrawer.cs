using PostEnot.EditorExtensions.Editor;
using PostEnot.Toolkits;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(WithoutLabelAttribute))]
    internal sealed class WithoutLabelAttributeDrawer : ModifyAttributeDrawer<WithoutLabelAttribute>
    {
        // Необходим именно AfterAttach, т.к. при установке label = string.Empty происходит изменение иерархии.
        private protected override void AfterAttach(
            SerializedProperty property,
            PropertyField propertyField,
            FieldInfo fieldInfo,
            WithoutLabelAttribute attribute)
        {
            if (propertyField != null)
            {
                propertyField.label = string.Empty;
            }
        }
    }
}