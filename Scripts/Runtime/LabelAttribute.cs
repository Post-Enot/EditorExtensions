namespace PostEnot.Toolkits
{
    public sealed class LabelAttribute : AdditionalPropertyAttribute
    {
        public LabelAttribute(string label) : base(true) => Label = label;

        public string Label { get; }
    }
}
