namespace PostEnot.Toolkits
{
    public sealed class ReadOnlyInEditorModeAttribute : ReadOnlyInAttribute
    {
        public ReadOnlyInEditorModeAttribute() : base(false) { }
    }
}
