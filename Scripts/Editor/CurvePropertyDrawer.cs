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
    internal class CurvePropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
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
                Debug.Log("SET");
                Type curveType = typeof(CurveField);
                FieldInfo fieldInfo = curveType.GetField("m_CurveColor", BindingFlags.NonPublic | BindingFlags.Instance);
                fieldInfo?.SetValue(curveField, color);
                curveField.MarkDirtyRepaint();
            }
        }
    }
}
