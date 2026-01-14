namespace PostEnot.Toolkits
{
    public sealed class WithoutLabelAttribute : LabelAttribute
    {
        public WithoutLabelAttribute() : base(string.Empty) {}
    }
}
