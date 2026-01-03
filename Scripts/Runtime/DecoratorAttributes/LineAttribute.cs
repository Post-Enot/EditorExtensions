namespace PostEnot.Toolkits
{
    public sealed class LineAttribute : DecoratorPropertyAttribute
    {
        public LineAttribute(AttributeDrawMode drawMode = AttributeDrawMode.Before) : base(drawMode) {}

        public LineAttribute(string hexColor, AttributeDrawMode drawMode = AttributeDrawMode.Before) : base(drawMode) => HexColor = hexColor;

        public LineAttribute(int heightInPixels, AttributeDrawMode drawMode = AttributeDrawMode.Before) : base(drawMode) => HeightInPixels = heightInPixels;

        public LineAttribute(string hexColor, int heightInPixels, AttributeDrawMode drawMode = AttributeDrawMode.Before) : base(drawMode)
        {
            HexColor = hexColor;
            HeightInPixels = heightInPixels;
        }

        public int HeightInPixels { get; }
        public string HexColor { get; }
    }
}
