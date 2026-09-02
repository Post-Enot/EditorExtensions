namespace PostEnot.Toolkits
{
    public sealed class DisableInPlayModeAttribute : DisableInAttribute
    {
        public DisableInPlayModeAttribute() : base(true) { }
    }
}
