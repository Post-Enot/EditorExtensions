using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    internal abstract class AdvancedPopupField<T> : PopupField<T>
    {
        internal AdvancedPopupField(string label) : base(label)
            => UIUtility.WrapGenericMenuCreation(this, OnCreateMenu, OnBeforeMenuShow);

        private protected virtual void OnCreateMenu() {}

        private protected virtual void OnBeforeMenuShow(AbstractGenericMenu menu) {}
    }
}
