using PostEnot.Toolkits;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(HelpBoxAttribute), true)]
    public sealed class HelpBoxAttributeDrawer : DecoratorDrawer
    {
        public override VisualElement CreatePropertyGUI()
        {
            VisualElement temp = new()
            {
                name = "TEMP",
                userData = attribute
            };
            temp.RegisterCallbackOnce<AttachToPanelEvent>(OnAttachToPanel);
            return temp;
        }

        private void OnAttachToPanel(AttachToPanelEvent context)
        {
            VisualElement temp = context.target as VisualElement;
            HelpBoxAttribute helpBoxAttribute = temp.userData as HelpBoxAttribute;
            temp.schedule.Execute(() => AfterAttach(helpBoxAttribute, temp));
        }

        private void AfterAttach(HelpBoxAttribute attribute, VisualElement temp)
        {
            PropertyField propertyField = temp.GetFirstAncestorOfType<PropertyField>();
            SerializedProperty serializedProperty = SerializationUtility.GetSerializedProperty(propertyField);
            FieldInfo fieldInfo = SerializationUtility.GetFieldInfo(serializedProperty);
            SerializedProperty parentSerializedProperty = SerializationUtility.GetParentProperty(serializedProperty);
            string methodName = attribute.MethodName;
            HelpBox helpBox = new(attribute.Text, attribute.MessageType);
            if (methodName != null)
            {
                helpBox.onButtonClicked += UIUtility.CreateActionFromMethodName(
                    serializedProperty,
                    parentSerializedProperty,
                    fieldInfo,
                    methodName,
                    out MethodInfo methodInfo);
                helpBox.buttonText = UIUtility.NicifyButtonName(attribute.ButtonText, methodInfo);
            }
            UIUtility.ApplyDrawMode(attribute.DrawMode, temp, helpBox);
        }
    }
}
