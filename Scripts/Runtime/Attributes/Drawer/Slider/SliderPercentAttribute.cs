namespace PostEnot.Toolkits
{
    public sealed class SliderPercentAttribute : SliderAttribute
    {
        public SliderPercentAttribute(bool showLabels = true)
            : base(0.0f, 1.0f, showLabels ? "0%" : string.Empty, showLabels ? "100%" : string.Empty) { }
    }
}
