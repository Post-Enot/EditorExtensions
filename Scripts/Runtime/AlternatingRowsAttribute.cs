namespace PostEnot.Toolkits
{
    public sealed class AlternatingRowsAttribute : AdditionalPropertyAttribute
    {
        public AlternatingRowsAttribute(bool contentOnly = false) : base(true) => ContentOnly = contentOnly;

        public bool ContentOnly { get; }
    }
}
