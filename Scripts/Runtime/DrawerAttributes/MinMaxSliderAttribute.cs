using UnityEngine;

namespace PostEnot.Toolkits
{
    public sealed class MinMaxSliderAttribute : PropertyAttribute
    {
        public MinMaxSliderAttribute() : this(0.0f, 1.0f) {}

        public MinMaxSliderAttribute(int min, int max) : this((float)min, max) {}

        public MinMaxSliderAttribute(float min, float max)
        {
            Min = Mathf.Min(min, max);
            Max = Mathf.Max(min, max);
        }

        public float Min { get; }
        public float Max { get; }
    }
}
