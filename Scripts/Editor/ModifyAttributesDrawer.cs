//using PostEnot.Toolkits;
//using System.Collections.Generic;
//using System.Reflection;
//using UnityEditor;
//using UnityEditor.UIElements;
//using UnityEngine;
//using UnityEngine.UIElements;

//namespace PostEnot.EditorExtensions.Editor
//{
//    [CustomPropertyDrawer(typeof(ModifyPropertyAttribute), true)]
//    internal sealed class ModifyAttributesDrawer : DecoratorDrawer
//    {
//        public override VisualElement CreatePropertyGUI()
//        {
//            VisualElement temp = new()
//            {
//                name = "TEMP"
//            };
//            temp.RegisterCallbackOnce<AttachToPanelEvent>(OnAttachToPanel);
//            return temp;
//        }

//        private void OnAttachToPanel(AttachToPanelEvent context)
//        {
//            VisualElement temp = (VisualElement)context.target;
//            temp.schedule.Execute(() => Draw(temp));
//        }

//        private void Draw(VisualElement temp)
//        {
//            PropertyField propertyField = temp.GetFirstAncestorOfType<PropertyField>();
//            SerializedProperty serializedProperty = SerializationUtility.GetSerializedProperty(propertyField);
//            FieldInfo fieldInfo = SerializationUtility.GetFieldInfo(serializedProperty);
//            if (fieldInfo.TryGetCustomAttribute(out VectorLabelsAttribute vectorLabelsAttribute))
//            {
//                if (serializedProperty.propertyType is SerializedPropertyType.Vector2)
//                {
//                    Vector2Field vectorField = propertyField.Q<Vector2Field>();
//                    ApplyLabelsForVector2Field<float>(vectorField, vectorLabelsAttribute);
//                }
//                else if (serializedProperty.propertyType is SerializedPropertyType.Vector2Int)
//                {
//                    Vector2IntField vectorField = propertyField.Q<Vector2IntField>();
//                    ApplyLabelsForVector2Field<int>(vectorField, vectorLabelsAttribute);
//                }
//                else if (serializedProperty.propertyType is SerializedPropertyType.Vector3)
//                {
//                    Vector3Field vectorField = propertyField.Q<Vector3Field>();
//                    ApplyLabelsForVector3Field<float>(vectorField, vectorLabelsAttribute);
//                }
//                else if (serializedProperty.propertyType is SerializedPropertyType.Vector3Int)
//                {
//                    Vector3IntField vectorField = propertyField.Q<Vector3IntField>();
//                    ApplyLabelsForVector3Field<int>(vectorField, vectorLabelsAttribute);
//                }
//            }
//        }

//        private static void ApplyLabelsForVector3Field<T>(VisualElement vectorField, VectorLabelsAttribute vectorLabelAttribute)
//        {
//            EditorAttributesSettingsAsset settings = EditorAttributesSettingsAsset.GetSettings();
//            vectorField.styleSheets.Add(settings.VectorLabelsStyleSheet);
//            vectorField.AddToClassList("pe-vector-labels");
//            TextValueField<T> xInputField = vectorField.Q<TextValueField<T>>("unity-x-input");
//            TextValueField<T> yInputField = vectorField.Q<TextValueField<T>>("unity-y-input");
//            TextValueField<T> zInputField = vectorField.Q<TextValueField<T>>("unity-z-input");
//            zInputField.AddToClassList("pe-vector-labels__last-composite-field");
//            xInputField.label = vectorLabelAttribute.XLabel;
//            yInputField.label = vectorLabelAttribute.YLabel;
//            zInputField.label = vectorLabelAttribute.ZLabel;
//            vectorField.schedule.Execute(() => ApplyWidth(vectorLabelAttribute, xInputField.labelElement, yInputField.labelElement, zInputField.labelElement));
//        }

//        private static void ApplyLabelsForVector2Field<T>(VisualElement vectorField, VectorLabelsAttribute vectorLabelAttribute)
//        {
//            EditorAttributesSettingsAsset settings = EditorAttributesSettingsAsset.GetSettings();
//            vectorField.styleSheets.Add(settings.VectorLabelsStyleSheet);
//            vectorField.AddToClassList("pe-vector-labels");
//            TextValueField<T> xInputField = vectorField.Q<TextValueField<T>>("unity-x-input");
//            TextValueField<T> yInputField = vectorField.Q<TextValueField<T>>("unity-y-input");
//            yInputField.AddToClassList("pe-vector-labels__last-composite-field");
//            xInputField.label = vectorLabelAttribute.XLabel;
//            yInputField.label = vectorLabelAttribute.YLabel;
//            vectorField.schedule.Execute(() => ApplyWidth(vectorLabelAttribute, xInputField.labelElement, yInputField.labelElement));
//        }

//        private static void ApplyWidth(VectorLabelsAttribute vectorLabelAttribute, Label xLabel, Label yLabel, Label zLabel = null)
//        {
//            //List<Label> labels = new();
//            //ApplyWidth(labels, xLabel, vectorLabelAttribute.XWidth);
//            //ApplyWidth(labels, yLabel, vectorLabelAttribute.YWidth);
//            //if (zLabel != null)
//            //{
//            //    ApplyWidth(labels, zLabel, vectorLabelAttribute.ZWidth);
//            //}
//            //float width = MeasureLabelsWidth(labels);
//            //foreach (Label label in labels)
//            //{
//            //    label.style.flexBasis = width;
//            //}
//        }

//        private static void ApplyWidth(List<Label> labels, Label label, int width)
//        {
//            if (string.IsNullOrEmpty(label.text))
//            {
//                return;
//            }
//            if (width <= 0)
//            {
//                labels.Add(label);
//                return;
//            }
//            label.style.flexBasis = width;
//        }

//        private static float MeasureLabelsWidth(List<Label> labels)
//        {
//            float maxWidth = 0;
//            foreach (Label label in labels)
//            {
//                Vector2 size = label.MeasureTextSize(
//                    label.text,
//                    float.PositiveInfinity,
//                    VisualElement.MeasureMode.Undefined,
//                    label.resolvedStyle.height,
//                    VisualElement.MeasureMode.Exactly);
//                float width = size.x + label.resolvedStyle.paddingLeft + label.resolvedStyle.paddingRight;
//                maxWidth = Mathf.Max(width, maxWidth);
//            }
//            return maxWidth;
//        }
//    }
//}
