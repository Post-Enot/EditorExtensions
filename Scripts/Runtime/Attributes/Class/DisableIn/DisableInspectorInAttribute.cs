namespace PostEnot.Toolkits
{
    public abstract class DisableInspectorInAttribute : ClassAttribute
    {
        internal DisableInspectorInAttribute(bool isEnabledInEditor) => IsEnabledInEditor = isEnabledInEditor;

        public bool IsEnabledInEditor { get; }
    }
}
