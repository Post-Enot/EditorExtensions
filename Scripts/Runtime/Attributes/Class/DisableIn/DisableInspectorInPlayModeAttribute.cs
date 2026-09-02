namespace PostEnot.Toolkits
{
    public sealed class DisableInspectorInPlayModeAttribute : DisableInspectorInAttribute
    {
        public DisableInspectorInPlayModeAttribute() : base(true) { }
    }
}
