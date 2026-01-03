namespace PostEnot.Toolkits
{
    public sealed class AlternatingRowsAttribute : ModifyPropertyAttribute
    {
        public AlternatingRowsAttribute(bool contentOnly = false) => ContentOnly = contentOnly;

        public bool ContentOnly { get; }
    }
}
