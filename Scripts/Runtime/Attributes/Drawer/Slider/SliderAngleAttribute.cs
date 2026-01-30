namespace PostEnot.Toolkits
{
    public sealed class SliderAngleAttribute : SliderAttribute
    {
        public SliderAngleAttribute(float minValue, float maxValue, bool showLabels = true)
            : base(minValue, maxValue, showLabels ? $"{minValue}°" : string.Empty, showLabels ? $"{maxValue}°" : string.Empty) { }
    }
}
