namespace PostEnot.Toolkits
{
    public sealed class LabelAttribute : AdditionalPropertyAttribute
    {
        public LabelAttribute(string label) => Label = label;

        public string Label { get; }
    }
}
