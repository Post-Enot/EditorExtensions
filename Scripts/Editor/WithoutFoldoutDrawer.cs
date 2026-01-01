using PostEnot.Toolkits;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(WithoutFoldoutAttribute))]
    public sealed class WithoutFoldoutDrawer : DecoratorDrawer
    {
        public override VisualElement CreatePropertyGUI()
        {
            VisualElement temp = new()
            {
                name = "TEMP",
            };
            temp.RegisterCallbackOnce<AttachToPanelEvent>(OnAttachToPanel);
            return temp;
        }

        private void OnAttachToPanel(AttachToPanelEvent context)
        {
            VisualElement temp = (VisualElement)context.target;
            PropertyField propertyField = temp.GetFirstAncestorOfType<PropertyField>();
            temp.userData = propertyField;
            temp.schedule.Execute(() => AfterOnAttach(propertyField));
        }

        private void AfterOnAttach(PropertyField propertyField)
        {
            SerializedProperty serializedProperty = SerializationUtility.GetSerializedProperty(propertyField);
            if (serializedProperty.isArray)
            {
                ListView listView = propertyField.Q<ListView>();
                listView.bindItem += BindItem;
            }
            else
            {
                RemoveFoldoutForElement(propertyField);
            }
        }

        private static void BindItem(VisualElement element, int index)
        {
            PropertyField propertyField = element as PropertyField;
            RemoveFoldoutForElement(propertyField);
        }

        private static void RemoveFoldoutForElement(PropertyField propertyField)
        {
            Foldout foldout = propertyField.Q<Foldout>();
            if (foldout == null)
            {
                Debug.Log(foldout);
                return;
            }
            foldout.value = true;
            List<VisualElement> children = new(foldout.Children());
            foreach (VisualElement child in children)
            {
                child.RemoveFromHierarchy();
                foldout.parent.Add(child);
            }
            propertyField.userData = foldout;
            foldout.userData = children;
            foldout.style.display = DisplayStyle.None;
        }
    }
}
