namespace PostEnot.Toolkits
{
    public sealed class AlternatingRowsAttribute : DecoratorPropertyAttribute
    {
        public AlternatingRowsAttribute(bool contentOnly = false) => ContentOnly = contentOnly;

        public bool ContentOnly { get; }
    }
}
