namespace PostEnot.Toolkits
{
    public sealed class DisableInspectorInEditorModeAttribute : DisableInspectorInAttribute
    {
        public DisableInspectorInEditorModeAttribute() : base(false) { }
    }
}
