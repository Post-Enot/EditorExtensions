namespace PostEnot.Toolkits
{
    public sealed class DisableInEditorModeAttribute : DisableInAttribute
    {
        public DisableInEditorModeAttribute() : base(true) { }
    }
}
