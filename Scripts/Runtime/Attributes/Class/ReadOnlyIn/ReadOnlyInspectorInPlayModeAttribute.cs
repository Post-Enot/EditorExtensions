namespace PostEnot.Toolkits
{
    public sealed class ReadOnlyInspectorInPlayModeAttribute : ReadOnlyInspectorInAttribute
    {
        public ReadOnlyInspectorInPlayModeAttribute() : base(true) { }
    }
}
