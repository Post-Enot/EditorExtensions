using UnityEngine;

namespace PostEnot.Toolkits
{
    public sealed class SliderAttribute : PropertyAttribute
    {
        public SliderAttribute(float min, float max)
        {
            MinX = min;
            MaxX = max;
        }

        public SliderAttribute(float min, float max, string minLabel, string maxLabel)
        {
            MinX = min;
            MaxX = max;
            MinLabel = minLabel;
            MaxLabel = maxLabel;
        }

        public float MinX { get; }
        public float MaxX { get; }
        public string MinLabel { get; }
        public string MaxLabel { get; }
    }
}
