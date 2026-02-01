namespace PostEnot.Toolkits
{
    public sealed class GetInChildrenOnlyAttribute : BaseGetAttribute
    {
        public GetInChildrenOnlyAttribute(bool includeInactive = false) => IncludeInactive = includeInactive;

        public bool IncludeInactive { get; }
    }
}
