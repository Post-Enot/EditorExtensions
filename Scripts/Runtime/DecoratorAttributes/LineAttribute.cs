using System;

namespace PostEnot.Toolkits
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public sealed class LineAttribute : DecoratorPropertyAttribute
    {
        public LineAttribute(AttributeDrawMode drawMode = AttributeDrawMode.Before) => DrawMode = drawMode;

        public LineAttribute(string hexColor, AttributeDrawMode drawMode = AttributeDrawMode.Before)
        {
            HexColor = hexColor;
            DrawMode = drawMode;
        }

        public LineAttribute(int heightInPixels, AttributeDrawMode drawMode = AttributeDrawMode.Before)
        {
            HeightInPixels = heightInPixels;
            DrawMode = drawMode;
        }

        public LineAttribute(string hexColor, int heightInPixels, AttributeDrawMode drawMode = AttributeDrawMode.Before)
        {
            HexColor = hexColor;
            HeightInPixels = heightInPixels;
            DrawMode = drawMode;
        }

        public int HeightInPixels { get; }
        public string HexColor { get; }
        public AttributeDrawMode DrawMode { get; }
    }
}
