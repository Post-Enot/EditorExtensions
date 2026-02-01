namespace PostEnot.Toolkits
{
    public sealed class GetInParentAttribute : BaseGetAttribute
    {
        public GetInParentAttribute(bool includeInactive = false) => IncludeInactive = includeInactive;

        public bool IncludeInactive { get; }
    }
}
