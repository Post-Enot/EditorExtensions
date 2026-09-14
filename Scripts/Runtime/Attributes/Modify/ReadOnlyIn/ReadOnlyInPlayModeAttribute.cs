namespace PostEnot.Toolkits
{
    public sealed class ReadOnlyInPlayModeAttribute : ReadOnlyInAttribute
    {
        public ReadOnlyInPlayModeAttribute() : base(true) { }
    }
}
