using System;
using PostEnot.Toolkits;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(SliderAttribute))]
    internal sealed class SliderAttributeDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SliderAttribute sliderAttribute = attribute as SliderAttribute;
            if (property.propertyType is SerializedPropertyType.Float)
            {
                Slider slider = new(sliderAttribute.MinX, sliderAttribute.MaxX)
                {
                    label = preferredLabel,
                    showInputField = true
                };
                ApplyStyle(slider, sliderAttribute);
                slider.BindProperty(property);
                return slider;
            }
            if (property.propertyType is SerializedPropertyType.Integer)
            {
                SliderInt slider = new((int)sliderAttribute.MinX, (int)sliderAttribute.MaxX)
                {
                    label = preferredLabel,
                    showInputField = true
                };
                ApplyStyle(slider, sliderAttribute);
                slider.BindProperty(property);
                return slider;
            }
            return new Label($"Use Slider with int or float.");
        }

        private static void ApplyStyle<TValue>(BaseSlider<TValue> slider, SliderAttribute attribute) where TValue : IComparable<TValue>
        {
            slider.AddToClassList(BaseField<float>.alignedFieldUssClassName);
            EditorAttributesSettingsAsset settingsAsset = EditorAttributesSettingsAsset.GetSettings();
            slider.styleSheets.Add(settingsAsset.SliderStyleSheet);
            if (!string.IsNullOrWhiteSpace(attribute.MinLabel) && !string.IsNullOrWhiteSpace(attribute.MaxLabel))
            {
                VisualElement dragContainer = slider.Q<VisualElement>("unity-drag-container");
                VisualElement container = new()
                {
                    name = "container"
                };
                Label miniLabelMin = new(attribute.MinLabel);
                Label miniLabelMax = new(attribute.MaxLabel);
                container.AddToClassList("pe-slider__mini-label-container");
                miniLabelMin.AddToClassList("pe-slider__mini-label");
                miniLabelMax.AddToClassList("pe-slider__mini-label");
                miniLabelMin.AddToClassList("pe-slider__mini-label-min");
                miniLabelMax.AddToClassList("pe-slider__mini-label-max");
                container.Add(miniLabelMin);
                container.Add(miniLabelMax);
                dragContainer.Add(container);
                container.style.flexDirection = FlexDirection.Row;
                container.style.justifyContent = Justify.SpaceBetween;

            }
        }
    }
}
