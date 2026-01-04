using PostEnot.Toolkits;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(VectorLabelsAttribute))]
    internal sealed class VectorLabelsAttributeDrawer : ModifyAttributeDrawer<VectorLabelsAttribute>
    {
        private protected override void OnAttach(
            SerializedProperty property,
            PropertyField propertyField,
            FieldInfo fieldInfo,
            VectorLabelsAttribute attribute)
        {
            if ((property == null) || (propertyField == null))
            {
                return;
            }
            if (property.propertyType is SerializedPropertyType.Vector2)
            {
                Vector2Field vectorField = propertyField.Q<Vector2Field>();
                ApplyStyle<float>(vectorField, attribute);
            }
            else if (property.propertyType is SerializedPropertyType.Vector2Int)
            {
                Vector2IntField vectorField = propertyField.Q<Vector2IntField>();
                ApplyStyle<int>(vectorField, attribute);
            }
            else if (property.propertyType is SerializedPropertyType.Vector3)
            {
                Vector3Field vectorField = propertyField.Q<Vector3Field>();
                ApplyStyle<float>(vectorField, attribute);
            }
            else if (property.propertyType is SerializedPropertyType.Vector3Int)
            {
                Vector3IntField vectorField = propertyField.Q<Vector3IntField>();
                ApplyStyle<int>(vectorField, attribute);
            }
        }

        private static void ApplyStyle<T>(VisualElement vectorField, VectorLabelsAttribute attribute)
        {
            if (vectorField == null)
            {
                return;
            }
            TextValueField<T> xAxisField = vectorField.Q<TextValueField<T>>("unity-x-input");
            TextValueField<T> yAxisField = vectorField.Q<TextValueField<T>>("unity-y-input");
            TextValueField<T> zAxisField = vectorField.Q<TextValueField<T>>("unity-z-input");
            SetLabel(xAxisField, attribute.XLabel);
            SetLabel(yAxisField, attribute.YLabel);
            SetLabel(zAxisField, attribute.ZLabel);
            vectorField.styleSheets.Add(Settings.VectorLabelsStyleSheet);
            vectorField.AddToClassList("pe-vector-labels");
            TextValueField<T> lastAxisField = zAxisField ?? yAxisField ?? xAxisField;
            lastAxisField?.AddToClassList("pe-vector-labels__last-composite-field");
            vectorField.schedule.Execute(() => ApplyWidth(attribute, xAxisField?.labelElement, yAxisField?.labelElement, zAxisField?.labelElement));
        }

        private static void SetLabel<T>(TextValueField<T> axisField, string label)
        {
            if (axisField == null)
            {
                return;
            }
            axisField.label = label;
        }

        private static void ApplyWidth(VectorLabelsAttribute attribute, Label xLabel, Label yLabel, Label zLabel = null)
        {
            List<Label> labels = new();
            ApplyWidth(labels, xLabel, attribute.XWidth);
            ApplyWidth(labels, yLabel, attribute.YWidth);
            ApplyWidth(labels, zLabel, attribute.ZWidth);
            float width = MeasureLabelsWidth(labels);
            foreach (Label label in labels)
            {
                label.style.flexBasis = width;
            }
        }

        private static void ApplyWidth(List<Label> labels, Label label, float width)
        {
            if (string.IsNullOrEmpty(label?.text) || (label?.parent == null))
            {
                return;
            }
            if (width > 0)
            {
                label.style.flexBasis = width;
                return;
            }
            labels.Add(label);
        }

        private static float MeasureLabelsWidth(List<Label> labels)
        {
            float maxWidth = 0;
            foreach (Label label in labels)
            {
                Vector2 size = label.MeasureTextSize(
                    label.text,
                    float.PositiveInfinity,
                    VisualElement.MeasureMode.Undefined,
                    label.resolvedStyle.height,
                    VisualElement.MeasureMode.Exactly);
                float width = size.x + label.resolvedStyle.paddingLeft + label.resolvedStyle.paddingRight;
                maxWidth = Mathf.Max(width, maxWidth);
            }
            return maxWidth;
        }
    }
}
