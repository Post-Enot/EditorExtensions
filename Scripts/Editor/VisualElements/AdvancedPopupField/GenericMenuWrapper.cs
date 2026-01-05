using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    internal sealed class GenericMenuWrapper : AbstractGenericMenu
    {
        public GenericMenuWrapper(VisualElement wrapped, Action<AbstractGenericMenu> beforeDropDownCallback)
        {
            if (wrapped == null)
            {
                throw new ArgumentNullException(nameof(wrapped));
            }
            if (beforeDropDownCallback == null)
            {
                throw new ArgumentNullException(nameof(beforeDropDownCallback));
            }
            _wrapped = wrapped;
            _beforeDropDownCallback = beforeDropDownCallback;
        }

        private AbstractGenericMenu Menu
        {
            get
            {
                _menu ??= _wrapped.panel.CreateMenu();
                return _menu;
            }
        }

        private readonly Action<AbstractGenericMenu> _beforeDropDownCallback;
        private readonly VisualElement _wrapped;

        private AbstractGenericMenu _menu;

        public override void AddDisabledItem(string itemName, bool isChecked)
            => Menu.AddDisabledItem(itemName, isChecked);

        public override void AddItem(string itemName, bool isChecked, Action action)
            => Menu.AddItem(itemName, isChecked, action);

        public override void AddItem(string itemName, bool isChecked, Action<object> action, object data)
            => Menu.AddItem(itemName, isChecked, action, data);

        public override void AddSeparator(string path) => Menu.AddSeparator(path);

        public override void DropDown(Rect position, VisualElement targetElement, DropdownMenuSizeMode dropdownMenuSizeMode = DropdownMenuSizeMode.Auto)
        {
            _beforeDropDownCallback?.Invoke(Menu);
            Menu.DropDown(position, targetElement, dropdownMenuSizeMode);
        }
    }
}
