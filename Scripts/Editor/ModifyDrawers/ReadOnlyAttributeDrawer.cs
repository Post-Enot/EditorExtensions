using PostEnot.Toolkits;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    internal sealed class ReadOnlyAttributeDrawer : ModifyAttributeDrawer<ReadOnlyAttribute>
    {
        private protected override void OnAttach(
            SerializedProperty property,
            PropertyField propertyField,
            FieldInfo fieldInfo,
            ReadOnlyAttribute attribute) => propertyField?.SetEnabled(false);
    }
}
