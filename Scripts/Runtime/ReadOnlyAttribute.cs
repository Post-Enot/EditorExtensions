namespace PostEnot.Toolkits
{
    public sealed class ReadOnlyAttribute : AdditionalPropertyAttribute
    {
        public ReadOnlyAttribute() : base(true) {}
    }
}
