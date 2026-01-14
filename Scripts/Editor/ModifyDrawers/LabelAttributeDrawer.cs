using PostEnot.Toolkits;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    internal sealed class LabelAttributeDrawer : AdvancedModificatorDrawer<LabelAttribute>
    {
        internal override void ModifyProperty(
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
