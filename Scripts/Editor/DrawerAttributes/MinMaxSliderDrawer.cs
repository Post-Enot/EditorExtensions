using PostEnot.Toolkits;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
    internal sealed class MinMaxSliderDrawer : BasePropertyDrawer<MinMaxSliderAttribute>
    {
        private protected override VisualElement CreateProperty(
            SerializedProperty property,
            FieldInfo fieldInfo,
            MinMaxSliderAttribute attribute)
        {
            if (property.propertyType is not SerializedPropertyType.Vector2)
            {
                return new Label($"Use MinMaxSlider with Vector2.");
            }
            AdvancedMinMaxSlider minMaxSlider = new(preferredLabel, attribute.Min, attribute.Max);
            minMaxSlider.BindProperty(property);
            minMaxSlider.styleSheets.Add(Settings.MinMaxSliderStyleSheet);
            minMaxSlider.AddToClassList(BaseField<object>.alignedFieldUssClassName);
            return minMaxSlider;
        }
    }
}
