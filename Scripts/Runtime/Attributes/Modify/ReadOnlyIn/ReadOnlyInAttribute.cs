namespace PostEnot.Toolkits
{
    public abstract class ReadOnlyInAttribute : ModifyPropertyAttribute
    {
        internal ReadOnlyInAttribute(bool isEnabledInEditor) => IsEnabledInEditor = isEnabledInEditor;

        public bool IsEnabledInEditor { get; }
    }
}
