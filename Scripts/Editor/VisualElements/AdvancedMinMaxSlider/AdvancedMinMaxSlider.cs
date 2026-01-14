using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    internal sealed class AdvancedMinMaxSlider : BaseField<Vector2>
    {
        public AdvancedMinMaxSlider(string label, float minLimit, float maxLimit) : base(label, null)
        {
            _minValueField = new FloatField
            {
                name = "min-value-field",
                formatString = "0.##",
                isDelayed = true
            };
            _maxValueField = new FloatField
            {
                name = "max-value-field",
                formatString = "0.##",
                isDelayed = true
            };
            _slider = new MinMaxSlider(0.0f, 0.0f, minLimit, maxLimit)
            {
                name = "slider"
            };
            _inputContainer = new VisualElement
            {
                name = "input"
            };
            _inputContainer.Add(_minValueField);
            _inputContainer.Add(_slider);
            _inputContainer.Add(_maxValueField);

            typeof(BaseField<Vector2>)
                .GetProperty("visualInput", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(this, _inputContainer);
            AddToClassList("pe-min-max-slider-field");

            _slider.RegisterValueChangedCallback(OnSliderValueChanged);
            _minValueField.RegisterValueChangedCallback(OnMinFieldValueChanged);
            _maxValueField.RegisterValueChangedCallback(OnMaxFieldValueChanged);
        }

        public float LowLimit => _slider.lowLimit;
        public float HighLimit => _slider.highLimit;

        private readonly VisualElement _inputContainer;
        private readonly FloatField _minValueField;
        private readonly FloatField _maxValueField;
        private readonly MinMaxSlider _slider;

        public override void SetValueWithoutNotify(Vector2 newValue)
        {
            base.SetValueWithoutNotify(newValue);
            _slider.SetValueWithoutNotify(newValue);
            _minValueField.SetValueWithoutNotify(newValue.x);
            _maxValueField.SetValueWithoutNotify(newValue.y);
        }

        private void OnSliderValueChanged(ChangeEvent<Vector2> context)
        {
            value = context.newValue;
            _minValueField.SetValueWithoutNotify(context.newValue.x);
            _maxValueField.SetValueWithoutNotify(context.newValue.y);
        }

        private void OnMaxFieldValueChanged(ChangeEvent<float> context)
        {
            float clampedValue = Mathf.Clamp(context.newValue, value.x, HighLimit);
            value = new Vector2(value.x, clampedValue);
            _maxValueField.SetValueWithoutNotify(clampedValue);
        }

        private void OnMinFieldValueChanged(ChangeEvent<float> context)
        {
            float clampedValue = Mathf.Clamp(context.newValue, LowLimit, value.y);
            value = new Vector2(clampedValue, value.y);
            _minValueField.SetValueWithoutNotify(clampedValue);
        }
    }
}
