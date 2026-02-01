namespace PostEnot.Toolkits
{
    public sealed class GetInHierarchyAttribute : BaseGetAttribute
    {
        public GetInHierarchyAttribute(bool includeInactive = false) => IncludeInactive = includeInactive;

        public bool IncludeInactive { get; }
    }
}
