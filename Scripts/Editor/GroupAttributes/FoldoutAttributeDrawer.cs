using PostEnot.Toolkits;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(FoldoutAttribute))]
    internal sealed class FoldoutAttributeDrawer : BaseGroupDrawer<FoldoutAttribute>
    {
        private protected override void AfterAttach(
            SerializedProperty property,
            PropertyField propertyField,
            FieldInfo fieldInfo,
            FoldoutAttribute attribute)
        {
            List<PropertyField> includedPropertyFields = new()
            {
                propertyField
            };
            Foldout foldout = new()
            {
                text = attribute.Name,
            };
            int index = propertyField.parent.IndexOf(propertyField);
            propertyField.parent.Insert(index, foldout);
            index += 2;
            for (int i = index; i < propertyField.parent.childCount; i += 1)
            {
                VisualElement visualElement = propertyField.parent.ElementAt(i);
                if (visualElement is PropertyField includedPropertyField)
                {
                    SerializedProperty includedSerializedProperty = SerializationUtility.GetSerializedProperty(includedPropertyField);
                    FieldInfo includedFieldInfo = SerializationUtility.GetFieldInfo(includedSerializedProperty);
                    if (includedFieldInfo.HasCustomAttribute<EndFoldoutAttribute>()
                        || includedFieldInfo.HasCustomAttribute<FoldoutAttribute>())
                    {
                        break;
                    }
                    includedPropertyFields.Add(includedPropertyField);
                }
            }
            foreach (PropertyField includedPropertyField in includedPropertyFields)
            {
                includedPropertyField.RemoveFromHierarchy();
                foldout.Add(includedPropertyField);
            }
        }
    }
}
