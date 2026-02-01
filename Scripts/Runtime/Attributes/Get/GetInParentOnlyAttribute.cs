namespace PostEnot.Toolkits
{
    public sealed class GetInParentOnlyAttribute : BaseGetAttribute
    {
        public GetInParentOnlyAttribute(bool includeInactive = false) => IncludeInactive = includeInactive;

        public bool IncludeInactive { get; }
    }
}
