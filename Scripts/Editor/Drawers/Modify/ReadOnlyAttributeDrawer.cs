using PostEnot.Toolkits;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    internal sealed class ReadOnlyAttributeDrawer : AdvancedModificatorDrawer<ReadOnlyAttribute>
    {
        internal override void ModifyProperty(
            SerializedProperty property,
            PropertyField propertyField,
            FieldInfo fieldInfo,
            ReadOnlyAttribute attribute) => propertyField.SetEnabled(false);
    }
}
