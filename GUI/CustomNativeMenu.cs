using GTA;
using LemonUI;
using LemonUI.Elements;
using LemonUI.Menus;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace FusionLibrary
{
    public delegate void OnItemSelected(NativeItem sender, SelectedEventArgs e);
    public delegate void OnItemActivated(NativeItem sender, EventArgs e);
    public delegate void OnItemCheckboxChanged(NativeCheckboxItem sender, EventArgs e, bool Checked);
    public delegate void OnItemValueChanged(NativeSliderItem sender, EventArgs e);

    public abstract class CustomNativeMenu : NativeMenu
    {
        public static ObjectPool ObjectPool { get; } = new ObjectPool();

        public static List<CustomNativeMenu> CustomNativeMenus { get; } = new List<CustomNativeMenu>();

        internal static void TickAll()
        {
            CustomNativeMenus.ForEach(x =>
            {
                if (x.Visible)
                {
                    x.Tick();
                }
            });
        }

        /// <summary>
        /// Event fired up when an item is activated.
        /// </summary>
        public event OnItemActivated OnItemActivated;

        /// <summary>
        /// Event fired up when an item is selected (got focused).
        /// </summary>
        public event OnItemSelected OnItemSelected;

        /// <summary>
        /// Event fired up when any checkbox item has its check state changed.
        /// </summary>
        public event OnItemCheckboxChanged OnItemCheckboxChanged;

        /// <summary>
        /// Event fired up when any list item selected value changed.
        /// </summary>
        public event OnItemValueChanged OnItemValueChanged;

        /// <summary>
        /// Internal name of the menu.
        /// </summary>
        public string InternalName { get; protected set; }

        private static readonly I2Dimensional defaultBanner = new ScaledTexture(PointF.Empty, new SizeF(0, 108), "commonmenu", "interaction_bgd");

        /// <summary>
        /// Instances a new menu with <paramref name="title"/>.
        /// </summary>
        /// <param name="title">Title of the menu.</param>
        public CustomNativeMenu(string title) : this(title, "", "", defaultBanner)
        {

        }

        /// <summary>
        /// Instances a new menu with <paramref name="title"/> and <paramref name="subtitle"/>.
        /// </summary>
        /// <param name="title">Title of the menu.</param>
        /// <param name="subtitle">Subtitle of the menu.</param>
        public CustomNativeMenu(string title, string subtitle) : this(title, subtitle, "", defaultBanner)
        {

        }

        /// <summary>
        /// Instances a new menu with <paramref name="title"/>, <paramref name="subtitle"/> and <paramref name="description"/>.
        /// </summary>
        /// <param name="title">Title of the menu.</param>
        /// <param name="subtitle">Subtitle of the menu.</param>
        /// <param name="description">Description of the menu.</param>
        public CustomNativeMenu(string title, string subtitle, string description) : this(title, subtitle, description, defaultBanner)
        {

        }

        /// <summary>
        /// Instances a new menu with <paramref name="title"/>, <paramref name="subtitle"/> and <paramref name="description"/> with <paramref name="banner"/>.
        /// </summary>
        /// <param name="title">Title of the menu.</param>
        /// <param name="subtitle">Subtitle of the menu.</param>
        /// <param name="description">Description of the menu.</param>
        /// <param name="banner">Banner of the menu.</param>
        public CustomNativeMenu(string title, string subtitle, string description, I2Dimensional banner) : base(title, subtitle, description, banner)
        {
            CustomNativeMenus.Add(this);
            ObjectPool.Add(this);

            Shown += Menu_Shown;
            Closing += Menu_Closing;
            OnItemActivated += Menu_OnItemActivated;
            OnItemSelected += Menu_OnItemSelected;
            OnItemCheckboxChanged += Menu_OnItemCheckboxChanged;
            OnItemValueChanged += Menu_OnItemValueChanged;
        }

        /// <summary>
        /// Default handler of <see cref="OnItemValueChanged"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public abstract void Menu_OnItemValueChanged(NativeSliderItem sender, EventArgs e);

        /// <summary>
        /// Default handler of <see cref="OnItemCheckboxChanged"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="Checked"></param>
        public abstract void Menu_OnItemCheckboxChanged(NativeCheckboxItem sender, EventArgs e, bool Checked);

        /// <summary>
        /// Default handler of <see cref="OnItemSelected"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public abstract void Menu_OnItemSelected(NativeItem sender, SelectedEventArgs e);

        /// <summary>
        /// Default handler of <see cref="OnItemActivated"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public abstract void Menu_OnItemActivated(NativeItem sender, EventArgs e);

        /// <summary>
        /// Default handler of <see cref="NativeMenu.Closing"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public abstract void Menu_Closing(object sender, System.ComponentModel.CancelEventArgs e);

        /// <summary>
        /// Default handler of <see cref="NativeMenu.Shown"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public abstract void Menu_Shown(object sender, EventArgs e);

        public new void Add(NativeItem nativeItem)
        {
            nativeItem.Activated += NativeItem_Activated;
            nativeItem.Selected += NativeItem_Selected;

            base.Add(nativeItem);
        }

        public new void Add(int position, NativeItem nativeItem)
        {
            nativeItem.Activated += NativeItem_Activated;
            nativeItem.Selected += NativeItem_Selected;

            base.Add(position, nativeItem);
        }

        public void Add(NativeCheckboxItem nativeCheckboxItem)
        {
            nativeCheckboxItem.Activated += NativeItem_Activated;
            nativeCheckboxItem.Selected += NativeItem_Selected;
            nativeCheckboxItem.CheckboxChanged += NativeCheckboxItem_CheckboxChanged;

            base.Add(nativeCheckboxItem);
        }

        public void Add<T>(NativeListItem<T> nativeListItem)
        {
            nativeListItem.Activated += NativeItem_Activated;
            nativeListItem.Selected += NativeItem_Selected;

            base.Add(nativeListItem);
        }

        public void Add(NativeSliderItem nativeSliderItem)
        {
            nativeSliderItem.Activated += NativeItem_Activated;
            nativeSliderItem.Selected += NativeItem_Selected;
            nativeSliderItem.ValueChanged += NativeSliderItem_ValueChanged;

            base.Add(nativeSliderItem);
        }

        public void Add(NativeSlidableItem nativeSlidableItem)
        {
            nativeSlidableItem.Activated += NativeItem_Activated;
            nativeSlidableItem.Selected += NativeItem_Selected;

            base.Add(nativeSlidableItem);
        }

        private void NativeSliderItem_ValueChanged(object sender, EventArgs e)
        {
            OnItemValueChanged?.Invoke((NativeSliderItem)sender, e);
        }

        private void NativeCheckboxItem_CheckboxChanged(object sender, EventArgs e)
        {
            if (!Game.IsControlJustPressed(Control.PhoneSelect))
            {
                return;
            }

            OnItemCheckboxChanged?.Invoke((NativeCheckboxItem)sender, e, ((NativeCheckboxItem)sender).Checked);
        }

        private void NativeItem_Selected(object sender, SelectedEventArgs e)
        {
            OnItemSelected?.Invoke(((NativeMenu)sender).SelectedItem, e);
        }

        private void NativeItem_Activated(object sender, EventArgs e)
        {
            OnItemActivated?.Invoke(((NativeMenu)sender).SelectedItem, e);
        }

        /// <summary>
        /// Adds a separator in the menu.
        /// </summary>
        /// <returns>A new instance of <see cref="NativeSeparatorItem"/>.</returns>
        public NativeSeparatorItem AddSeparator()
        {
            NativeSeparatorItem nativeSeparatorItem;

            Add(nativeSeparatorItem = new NativeSeparatorItem());

            return nativeSeparatorItem;
        }

        public abstract void Tick();

        /// <summary>
        /// Abstract method for return the localized menu title.
        /// </summary>
        /// <returns>Localized menu title.</returns>
        public abstract string GetMenuTitle();

        /// <summary>
        /// Abstract method for return the localized menu description.
        /// </summary>
        /// <returns>Localized menu description.</returns>
        public abstract string GetMenuDescription();

        /// <summary>
        /// Abstract method for return a localized item title.
        /// </summary>
        /// <param name="itemName">Item name to be localized.</param>
        /// <returns>Localized item name.</returns>
        public abstract string GetItemTitle(string itemName);

        /// <summary>
        /// Abstract method for return a localized item description.
        /// </summary>
        /// <param name="itemName">Item name to be description.</param>
        /// <returns>Localized item description.</returns>
        public abstract string GetItemDescription(string itemName);

        /// <summary>
        /// Abstract method for return a localized title value of a list item.
        /// </summary>
        /// <param name="itemName">Item name.</param>
        /// <param name="valueName">Value name.</param>
        /// <returns>Localized title value.</returns>
        public abstract string GetItemValueTitle(string itemName, string valueName);

        /// <summary>
        /// Abstract method for return a localized description value of a list item.
        /// </summary>
        /// <param name="itemName">Item name.</param>
        /// <param name="valueName">Value name.</param>
        /// <returns>Localized description value.</returns>
        public abstract string GetItemValueDescription(string itemName, string valueName);

        /// <summary>
        /// Abstract method for return a localized title value of a <see cref="NativeItem"/>.
        /// </summary>
        /// <param name="sender">Instance of a <see cref="NativeItem"/>.</param>
        /// <param name="valueName">Value name.</param>
        /// <returns>Localized title value.</returns>
        public string GetItemValueTitle(object sender, string valueName)
        {
            return GetItemValueTitle(((NativeItem)sender).Tag.ToString(), valueName);
        }

        /// <summary>
        /// Abstract method for return a localized title value of a <see cref="NativeItem"/>.
        /// </summary>
        /// <param name="itemName">Item name.</param>
        /// <param name="valueNames">List of value names.</param>
        /// <returns>Localized title values.</returns>
        public string[] GetItemValueTitle(string itemName, params string[] valueNames)
        {
            string[] ret = new string[valueNames.Length];

            for (int i = 0; i < valueNames.Length; i++)
            {
                ret[i] = GetItemValueTitle(itemName, valueNames[i]);
            }

            return ret;
        }

        /// <summary>
        /// Abstract method for return a localized description value of a <see cref="NativeItem"/>.
        /// </summary>
        /// <param name="sender">Instance of a <see cref="NativeItem"/>.</param>
        /// <param name="valueName">Value name.</param>
        /// <returns>Localized description value.</returns>
        public string GetItemValueDescription(object sender, string valueName)
        {
            return GetItemValueDescription(((NativeItem)sender).Tag.ToString(), valueName);
        }

        /// <summary>
        /// Abstract method for return a localized description value of a <see cref="NativeItem"/>.
        /// </summary>
        /// <param name="itemName">Item name.</param>
        /// <param name="valueNames">List of value names.</param>
        /// <returns>Localized description values.</returns>
        public string[] GetItemValueDescription(string itemName, params string[] valueNames)
        {
            string[] ret = new string[valueNames.Length];

            for (int i = 0; i < valueNames.Length; i++)
            {
                ret[i] = GetItemValueDescription(itemName, valueNames[i]);
            }

            return ret;
        }

        /// <summary>
        /// Adds a new <see cref="NativeSubmenuItem"/> using text localization system.
        /// </summary>
        /// <param name="menu">Instance of a<see cref="NativeMenu"/>.</param>
        /// <param name="menuName">Entry name.</param>
        /// <returns>Instance of the new <see cref="NativeSubmenuItem"/>.</returns>
        public NativeSubmenuItem NewSubmenu(NativeMenu menu, string menuName)
        {
            NativeSubmenuItem item = AddSubMenu(menu);
            item.Title = GetItemTitle(menuName);
            item.Description = GetItemDescription(menuName);

            item.Tag = menuName;

            return item;
        }

        /// <summary>
        /// Adds a new <see cref="NativeCheckboxItem"/> using text localization system.
        /// </summary>
        /// <param name="itemName">Entry name.</param>
        /// <returns>Instance of the new <see cref="NativeCheckboxItem"/>.</returns>
        public NativeCheckboxItem NewCheckboxItem(string itemName)
        {
            NativeCheckboxItem item;

            Add(item = new NativeCheckboxItem(GetItemTitle(itemName), GetItemDescription(itemName)));

            item.Tag = itemName;

            return item;
        }

        /// <summary>
        /// Adds a new <see cref="NativeCheckboxItem"/> using text localization system. With <paramref name="isChecked"/> default state.
        /// </summary>
        /// <param name="itemName">Entry name.</param>
        /// <param name="isChecked">Default state of checkbox.</param>
        /// <returns>Instance of the new <see cref="NativeCheckboxItem"/>.</returns>
        public NativeCheckboxItem NewCheckboxItem(string itemName, bool isChecked)
        {
            NativeCheckboxItem item;

            Add(item = new NativeCheckboxItem(GetItemTitle(itemName), GetItemDescription(itemName), isChecked));

            item.Tag = itemName;

            return item;
        }

        public NativeListItem<T> NewListItem<T>(string itemName, params T[] itemValues)
        {
            NativeListItem<T> item;

            Add(item = new NativeListItem<T>(GetItemTitle(itemName), GetItemDescription(itemName), itemValues));

            item.Tag = itemName;

            return item;
        }

        public NativeListItem<string> NewLocalizedListItem(string itemName, params string[] itemValues)
        {
            NativeListItem<string> item;

            Add(item = new NativeListItem<string>(GetItemTitle(itemName), GetItemDescription(itemName), GetItemValueTitle(itemName, itemValues)));

            item.Tag = itemName;

            return item;
        }

        public NativeListItem<T> NewListItem<T>(string itemName)
        {
            NativeListItem<T> item;

            Add(item = new NativeListItem<T>(GetItemTitle(itemName), GetItemDescription(itemName)));

            item.Tag = itemName;

            return item;
        }

        public NativeSliderItem NewSliderItem(string itemName, int max, int value)
        {
            NativeSliderItem item;

            Add(item = new NativeSliderItem(GetItemTitle(itemName), GetItemDescription(itemName), max, value));

            item.Tag = itemName;

            return item;
        }

        /// <summary>
        /// Adds a new <see cref="NativeItem"/> using text localization system.
        /// </summary>
        /// <param name="itemName">Item name.</param>
        /// <returns>New instance of <see cref="NativeItem"/>.</returns>
        public NativeItem NewItem(string itemName)
        {
            NativeItem item;

            Add(item = new NativeItem(GetItemTitle(itemName), GetItemDescription(itemName)));

            item.Tag = itemName;

            return item;
        }

        /// <summary>
        /// Adds a new <see cref="NativeItem"/> at <paramref name="pos"/> using text localization system.
        /// </summary>
        /// <param name="pos">Position of the new item.</param>
        /// <param name="itemName">Item name.</param>
        /// <returns>New instance of <see cref="NativeItem"/>.</returns>
        public NativeItem NewItem(int pos, string itemName)
        {
            NativeItem item;

            Add(pos, item = new NativeItem(GetItemTitle(itemName), GetItemDescription(itemName)));

            item.Tag = itemName;

            return item;
        }

        /// <summary>
        /// Removes <paramref name="nativeItem"/> from this menu.
        /// </summary>
        /// <param name="nativeItem"><see cref="NativeItem"/> to be removed.</param>
        public new void Remove(NativeItem nativeItem)
        {
            nativeItem.Activated -= NativeItem_Activated;
            nativeItem.Selected -= NativeItem_Selected;

            try
            {
                ((NativeSliderItem)nativeItem).ValueChanged -= NativeSliderItem_ValueChanged;
            } 
            catch
            {

            }
            
            try
            {
                ((NativeCheckboxItem)nativeItem).CheckboxChanged -= NativeCheckboxItem_CheckboxChanged;
            } 
            catch
            {

            }

            base.Remove(nativeItem);
        }
    }
}