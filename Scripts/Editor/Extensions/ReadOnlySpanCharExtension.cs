using System;

namespace PostEnot.EditorExtensions.Editor
{
    internal static class ReadOnlySpanCharExtension
    {
        internal static int LastIndexOf(this ReadOnlySpan<char> self, char value, int startIndex)
        {
            ReadOnlySpan<char> slice = self[..(startIndex + 1)];
            return slice.LastIndexOf(value);
        }
    }
}
