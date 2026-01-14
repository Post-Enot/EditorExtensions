using System;
using UnityEngine;

namespace PostEnot.Toolkits
{
    public sealed class TogglableListAttribute : PropertyAttribute
    {
        public TogglableListAttribute(Type enumType, string foldout = default) : base(true)
        {
            if (enumType == null)
            {
                throw new ArgumentNullException(nameof(enumType));
            }
            if (!enumType.IsEnum)
            {
                throw new ArgumentNullException();
            }
            EnumType = enumType;
            Foldout = foldout;
        }

        public Type EnumType { get; }
        public string Foldout { get; }
    }
}
