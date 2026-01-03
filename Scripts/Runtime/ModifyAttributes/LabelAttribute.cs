namespace PostEnot.Toolkits
{
    public sealed class LabelAttribute : ModifyPropertyAttribute
    {
        public LabelAttribute(string label) => Label = label;

        public string Label { get; }
    }
}
