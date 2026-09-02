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
            bool isEnabled = attribute.InEditor ^ EditorApplication.isPlayingOrWillChangePlaymode;
            propertyField.SetEnabled(isEnabled);
        }
    }
}
