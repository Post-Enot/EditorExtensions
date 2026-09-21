using PostEnot.Toolkits;
using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(FieldButtonAttribute))]
    internal sealed class FieldButtonAttributeDrawer : DecoratorDrawer
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
            FieldButtonAttribute attribute = temp.userData as FieldButtonAttribute;
            temp.schedule.Execute(() => OnAfterAttach(attribute, temp));
        }

        private void OnAfterAttach(FieldButtonAttribute attribute, VisualElement temp)
        {
            PropertyField propertyField = temp.GetFirstAncestorOfType<PropertyField>();
            SerializedProperty serializedProperty = SerializationUtility.GetSerializedProperty(propertyField);
            FieldInfo fieldInfo = SerializationUtility.GetFieldInfo(serializedProperty);
            SerializedProperty parentSerializedProperty = SerializationUtility.GetParentProperty(serializedProperty);
            string methodName = attribute.MethodName;

            Button button = new();
            button.style.marginLeft = 0;
            button.style.marginRight = 0;
            Action action = UIUtility.CreateActionFromMethodName(
                serializedProperty,
                parentSerializedProperty,
                fieldInfo,
                methodName,
                out MethodInfo methodInfo);
            button.text = UIUtility.NicifyButtonName(attribute.Text, methodInfo);
            button.clickable = new Clickable(action);
            if (attribute.Icon != null)
            {
                Texture2D texture = UIUtility.LoadIcon(attribute.Icon);
                button.iconImage = Background.FromTexture2D(texture);
            }
            UIUtility.ApplyDrawMode(attribute.DrawMode, temp, button);
            temp.RegisterCallbackOnce<DetachFromPanelEvent>(_ => button.RemoveFromHierarchy());
        }
    }
}
