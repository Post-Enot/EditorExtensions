namespace PostEnot.Toolkits
{
    public abstract class ReadOnlyInspectorInAttribute : ClassAttribute
    {
        internal ReadOnlyInspectorInAttribute(bool isEnabledInEditor) => IsEnabledInEditor = isEnabledInEditor;

        public bool IsEnabledInEditor { get; }
    }
}
