namespace PostEnot.Toolkits
{
    public class LabelAttribute : ModifyPropertyAttribute
    {
        public LabelAttribute(string label) => Label = label;

        public string Label { get; }
    }
}
