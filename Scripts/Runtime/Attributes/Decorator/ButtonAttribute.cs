namespace PostEnot.Toolkits
{
    public sealed class ButtonAttribute : DecoratorPropertyAttribute
    {
        public ButtonAttribute(string text, string methodName, AttributeDrawMode drawMode = AttributeDrawMode.Before) : base(drawMode)
        {
            Text = text;
            MethodName = methodName;
        }

        public string Text { get; }
        public string MethodName { get; }
    }
}
