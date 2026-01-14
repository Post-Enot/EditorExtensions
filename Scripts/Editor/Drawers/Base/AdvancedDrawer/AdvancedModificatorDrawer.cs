using PostEnot.Toolkits;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    internal abstract class AdvancedModificatorDrawer
    {
        private protected static EditorAttributesSettingsAsset Settings => EditorAttributesSettingsAsset.GetSettings();

        internal abstract void ModifyProperty(
            SerializedProperty property,
            PropertyField propertyField,
            FieldInfo fieldInfo,
            ModifyPropertyAttribute attribute);
    }
}
