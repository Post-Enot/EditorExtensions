namespace PostEnot.Toolkits
{
    public abstract class DisableInAttribute : ModifyPropertyAttribute
    {
        internal DisableInAttribute(bool inEditor) => InEditor = inEditor;

        public bool InEditor { get; }
    }
}
