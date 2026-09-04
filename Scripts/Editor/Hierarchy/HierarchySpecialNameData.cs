using PostEnot.Toolkits;
using System;
using UnityEngine;

namespace PostEnot.EditorExtensions.Editor
{
    [Serializable]
    public struct HierarchySpecialNameData
    {
        #region Inspector
        [SerializeField, Placeholder("Pattern...")] private string pattern;
        [SerializeField] private HierarchySpecialNameMode mode;
        [SerializeField] private Color color;
        [SerializeField] private bool isBold;
        #endregion

        public readonly string Pattern => pattern;
        public readonly HierarchySpecialNameMode Mode => mode;
        public readonly Color Color => color;
        public readonly bool IsBold => isBold;
    }
}
