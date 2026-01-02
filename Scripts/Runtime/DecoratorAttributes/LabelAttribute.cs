namespace PostEnot.Toolkits
{
    public sealed class LabelAttribute : DecoratorPropertyAttribute
    {
        public LabelAttribute(string label) => Label = label;

        public string Label { get; }
    }
}
