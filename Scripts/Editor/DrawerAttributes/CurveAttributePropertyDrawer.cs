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
    internal class CurveAttributePropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            if (property.propertyType is not SerializedPropertyType.AnimationCurve)
            {
                return new Label($"Use Curve with UnityEngine.AnimationCurve.");
            }
            CurveField curveField = new(preferredLabel);
            curveField.BindProperty(property);
            CurveAttribute curveAttribute = attribute as CurveAttribute;
            curveField.ranges = Rect.MinMaxRect(
                curveAttribute.MinX,
                curveAttribute.MinY,
                curveAttribute.MaxX,
                curveAttribute.MaxY);
            TrySetCurveColor(curveField, curveAttribute.HexColor);
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
