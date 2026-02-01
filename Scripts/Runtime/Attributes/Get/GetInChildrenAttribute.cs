namespace PostEnot.Toolkits
{
    public sealed class GetInChildrenAttribute : BaseGetAttribute
    {
        public GetInChildrenAttribute(bool includeInactive = false) => IncludeInactive = includeInactive;

        public bool IncludeInactive { get; }
    }
}
