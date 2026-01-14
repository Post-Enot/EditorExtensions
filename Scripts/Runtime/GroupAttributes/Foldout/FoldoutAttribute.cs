namespace PostEnot.Toolkits
{
    public sealed class FoldoutAttribute : GroupPropertyAttribute
    {
        public FoldoutAttribute(string name) => Name = name;

        public string Name { get; }
    }
}
