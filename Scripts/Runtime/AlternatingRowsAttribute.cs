namespace PostEnot.Toolkits
{
    public sealed class AlternatingRowsAttribute : AdditionalPropertyAttribute
    {
        public AlternatingRowsAttribute(bool contentOnly = false) => ContentOnly = contentOnly;

        public bool ContentOnly { get; }
    }
}
