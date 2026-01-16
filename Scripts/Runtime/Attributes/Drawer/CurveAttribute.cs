using UnityEngine;

namespace PostEnot.Toolkits
{
    public sealed class CurveAttribute : PropertyAttribute
    {
        public CurveAttribute()
        {
            MinX = 0.0f;
            MinY = 0.0f;
            MaxX = 1.0f;
            MaxY = 1.0f;
        }

        public CurveAttribute(string hexColor) => HexColor = hexColor;

        public CurveAttribute(float minX, float minY, float maxX, float maxY, string hexColor = default)
        {
            MinX = minX;
            MinY = minY;
            MaxX = maxX;
            MaxY = maxY;
            HexColor = hexColor;
        }

        public float MinX { get; }
        public float MinY { get; }
        public float MaxX { get; }
        public float MaxY { get; }
        public string HexColor { get; }
    }
}
