using PostEnot.Toolkits;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(DisableInAttribute), true)]
    internal sealed class DisableInAttributeDrawer : ModifyAttributeDrawer<DisableInAttribute>
    {
        private protected override void OnAttach(
            SerializedProperty property,
            PropertyField propertyField,
            FieldInfo fieldInfo,
            DisableInAttribute attribute)
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
