#nullable enable

namespace PostEnot.Toolkits
{
    public sealed class ButtonAttribute : MethodAttribute
    {
        public ButtonAttribute() { }

        public ButtonAttribute(string? text) => Text = text;

        public string? Text { get; }
    }
}
