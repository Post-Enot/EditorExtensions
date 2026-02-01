namespace PostEnot.Toolkits
{
    public sealed class GetInHierarchyOnlyAttribute : BaseGetAttribute
    {
        public GetInHierarchyOnlyAttribute(bool includeInactive = false) => IncludeInactive = includeInactive;

        public bool IncludeInactive { get; }
    }
}
