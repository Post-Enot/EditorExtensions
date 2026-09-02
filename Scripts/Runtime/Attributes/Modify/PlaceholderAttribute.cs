namespace PostEnot.Toolkits
{
    public sealed class PlaceholderAttribute : ModifyPropertyAttribute
    {
        public PlaceholderAttribute(string text) => Text = text;

        public string Text { get; }
    }
}
