namespace PostEnot.Toolkits
{
    public sealed class FieldButtonAttribute : DecoratorPropertyAttribute
    {
        public FieldButtonAttribute(string methodName, AttributeDrawMode drawMode = AttributeDrawMode.Before) : base(drawMode)
        {
            Text = null;
            MethodName = methodName;
        }

        public FieldButtonAttribute(string text, string methodName, AttributeDrawMode drawMode = AttributeDrawMode.Before) : base(drawMode)
        {
            Text = text;
            MethodName = methodName;
        }

        public string Text { get; }
        public string MethodName { get; }
        public string Icon { get; set; }
    }
}
