using PostEnot.Toolkits;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(AdditionalPropertyAttribute), true)]
    internal sealed class AdditionalPropertyAttributeDrawer : DecoratorDrawer
    {
        public override VisualElement CreatePropertyGUI()
        {
            VisualElement temp = new()
            {
                name = "TEMP"
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
            PropertyField propertyField = temp.GetFirstAncestorOfType<PropertyField>();
            SerializedProperty serializedProperty = SerializationUtility.GetSerializedProperty(propertyField);
            FieldInfo fieldInfo = SerializationUtility.GetFieldInfo(serializedProperty);
            if (fieldInfo.HasCustomAttribute<ReadOnlyAttribute>())
            {
                propertyField.SetEnabled(false);
            }
            if (fieldInfo.TryGetCustomAttribute(out LabelAttribute labelAttribute))
            {
                propertyField.label = labelAttribute.Label;
            }

            if (serializedProperty.isArray)
            {
                List<ListView> listViews = propertyField.Query<ListView>().Visible().ToList();
                if (fieldInfo.TryGetCustomAttribute(out AlternatingRowsAttribute alternatingRowsAttribute))
                {
                    foreach (ListView listView in listViews)
                    {
                        listView.showAlternatingRowBackgrounds = alternatingRowsAttribute.ContentOnly
                            ? AlternatingRowBackground.ContentOnly
                            : AlternatingRowBackground.All;
                    }
                }
            }
        }
    }
}
