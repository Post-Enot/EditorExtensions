using System;
using UnityEngine;

namespace PostEnot.Toolkits
{
    public sealed class PreviewAttribute : PropertyAttribute
    {
        public PreviewAttribute(PreviewSize size = PreviewSize.Default)
        {
            SizeInPixels = size switch
            {
                PreviewSize.Small => 60,
                PreviewSize.Medium or PreviewSize.Default => 90,
                PreviewSize.Big => 120,
                PreviewSize.Large => 160,
                _ => throw new ArgumentOutOfRangeException(nameof(size))
            };
        }

        public PreviewAttribute(int sizeInPixels) => SizeInPixels = sizeInPixels;

        public int SizeInPixels { get; }
    }
}
