namespace PostEnot.Toolkits
{
    public abstract class DisableInAttribute : ModifyPropertyAttribute
    {
        internal DisableInAttribute(bool isEnabledInEditor) => IsEnabledInEditor = isEnabledInEditor;

        public bool IsEnabledInEditor { get; }
    }
}
