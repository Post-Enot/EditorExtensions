using PostEnot.Toolkits;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    internal abstract class AdvancedModificatorDrawer<TAttribute> : AdvancedModificatorDrawer where TAttribute : ModifyPropertyAttribute
    {
        internal sealed override void ModifyProperty(
            SerializedProperty property,
            PropertyField propertyField,
            FieldInfo fieldInfo,
            ModifyPropertyAttribute attribute)
        {
            TAttribute castedAttribute = attribute as TAttribute;
            ModifyProperty(property, propertyField, fieldInfo, castedAttribute);
        }

        internal abstract void ModifyProperty(
            SerializedProperty property,
            PropertyField propertyField,
            FieldInfo fieldInfo,
            TAttribute attribute);
    }
}
