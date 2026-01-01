using PostEnot.Toolkits;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(FoldoutAttribute))]
    internal sealed class FoldoutDecoratorDrawer : DecoratorDrawer
    {
        public override VisualElement CreatePropertyGUI()
        {
            FoldoutAttribute foldoutAttribute = attribute as FoldoutAttribute;
            VisualElement temp = new()
            {
                name = "TEMP",
                userData = foldoutAttribute.Text
            };
            temp.RegisterCallbackOnce<AttachToPanelEvent>(OnAttachToPanel);
            return temp;
        }

        private void OnAttachToPanel(AttachToPanelEvent context)
        {
            VisualElement temp = (VisualElement)context.target;
            temp.schedule.Execute(() => Draw(temp));
        }

        private void Draw(VisualElement temp)
        {
            string text = temp.userData as string;
            List<PropertyField> propertyFields = new();
            PropertyField firstPropertyField = temp.GetFirstAncestorOfType<PropertyField>();
            propertyFields.Add(firstPropertyField);
            Foldout foldout = new()
            {
                text = text,
            };
            int index = firstPropertyField.parent.IndexOf(firstPropertyField);
            firstPropertyField.parent.Insert(index, foldout);
            index += 2;
            for (int i = index; i < firstPropertyField.parent.childCount; i += 1)
            {
                VisualElement visualElement = firstPropertyField.parent.ElementAt(i);
                if (visualElement is PropertyField propertyField)
                {
                    SerializedProperty serializedProperty = SerializationUtility.GetSerializedProperty(propertyField);
                    FieldInfo fieldInfo = SerializationUtility.GetFieldInfo(serializedProperty);
                    EndFoldoutAttribute endFoldoutAttribute = fieldInfo.GetCustomAttribute<EndFoldoutAttribute>();
                    if (endFoldoutAttribute != null)
                    {
                        break;
                    }
                    FoldoutAttribute foldoutAttribute = fieldInfo.GetCustomAttribute<FoldoutAttribute>();
                    if (foldoutAttribute != null)
                    {
                        break;
                    }
                    propertyFields.Add(propertyField);
                }
            }
            foreach (PropertyField propertyField1 in propertyFields)
            {
                propertyField1.RemoveFromHierarchy();
                foldout.Add(propertyField1);
            }
        }
    }
}
