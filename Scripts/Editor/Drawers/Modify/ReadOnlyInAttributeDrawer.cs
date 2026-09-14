using PostEnot.Toolkits;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(ReadOnlyInAttribute), true)]
    internal sealed class ReadOnlyInAttributeDrawer : ModifyAttributeDrawer<ReadOnlyInAttribute>
    {
        private protected override void OnAttach(
            SerializedProperty property,
            PropertyField propertyField,
            FieldInfo fieldInfo,
            ReadOnlyInAttribute attribute)
        {
            if (attribute == null)
            {
                return;
            }
            if (propertyField == null)
            {
                return;
            }
            bool isEnabled = attribute.IsEnabledInEditor ^ EditorApplication.isPlayingOrWillChangePlaymode;
            propertyField.SetEnabled(isEnabled);
        }
    }
}
