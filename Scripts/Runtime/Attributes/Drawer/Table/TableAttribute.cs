using UnityEngine;
using UnityEngine.UIElements;

namespace PostEnot.Toolkits
{
    public sealed class TableAttribute : PropertyAttribute
    {
        public TableAttribute() : base(true) { }

        public AlternatingRowBackground AlternatingRows { get; set; } = AlternatingRowBackground.ContentOnly;
        public bool Reorderable { get; set; } = true;
        public ListViewReorderMode ReorderMode { get; set; } = ListViewReorderMode.Animated;
        public bool ShowAddRemoveFooter { get; set; } = true;
        public bool ShowFoldout { get; set; } = true;
        public bool ShowBoundCollectionSize { get; set; } = true;
        public bool ShowBorder { get; set; } = true;
        public bool AllowAdd { get; set; } = true;
        public bool AllowRemove { get; set; } = true;
    }
}
