using PostEnot.Toolkits;
using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(CurveAttribute))]
    internal class CurveAttributeDrawer : BasePropertyDrawer<CurveAttribute>
    {
        private protected override VisualElement CreateProperty(
            SerializedProperty property,
            FieldInfo fieldInfo,
            CurveAttribute attribute)
        {
            if (property.propertyType is not SerializedPropertyType.AnimationCurve)
            {
                return new Label($"Use Curve with UnityEngine.AnimationCurve.");
            }
            CurveField curveField = new(preferredLabel);
            curveField.BindProperty(property);
            curveField.ranges = Rect.MinMaxRect(attribute.MinX, attribute.MinY, attribute.MaxX, attribute.MaxY);
            TrySetCurveColor(curveField, attribute.HexColor);
            curveField.AddToClassList(BaseField<AnimationCurve>.alignedFieldUssClassName);
            return curveField;
        }

        private static void TrySetCurveColor(CurveField curveField, string hexColor)
        {
            if (ColorUtility.TryParseHtmlString(hexColor, out Color color))
            {
                Type curveType = typeof(CurveField);
                FieldInfo fieldInfo = curveType.GetField("m_CurveColor", BindingFlags.NonPublic | BindingFlags.Instance);
                fieldInfo?.SetValue(curveField, color);
                curveField.MarkDirtyRepaint();
            }
        }
    }
}
