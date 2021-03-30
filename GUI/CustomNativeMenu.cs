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
        public static ObjectPool ObjectPool { get; internal set; } = new ObjectPool();

        public static List<CustomNativeMenu> CustomNativeMenus = new List<CustomNativeMenu>();

        internal static void TickAll()
        {
            CustomNativeMenus.ForEach(x =>
            {
                if (x.Visible)
                    x.Tick();
            });
        }

        public event OnItemActivated OnItemActivated;
        public event OnItemSelected OnItemSelected;
        public event OnItemCheckboxChanged OnItemCheckboxChanged;
        public event OnItemValueChanged OnItemValueChanged;

        public string InternalName { get; protected set; }

        private static I2Dimensional defaultBanner = new ScaledTexture(PointF.Empty, new SizeF(0, 108), "commonmenu", "interaction_bgd");

        public CustomNativeMenu(string title) : this(title, "", "", defaultBanner)
        {

        }

        public CustomNativeMenu(string title, string subtitle) : this(title, subtitle, "", defaultBanner)
        {

        }

        public CustomNativeMenu(string title, string subtitle, string description) : this(title, subtitle, description, defaultBanner)
        {

        }

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

        public abstract void Menu_OnItemValueChanged(NativeSliderItem sender, EventArgs e);

        public abstract void Menu_OnItemCheckboxChanged(NativeCheckboxItem sender, EventArgs e, bool Checked);

        public abstract void Menu_OnItemSelected(NativeItem sender, SelectedEventArgs e);

        public abstract void Menu_OnItemActivated(NativeItem sender, EventArgs e);

        public abstract void Menu_Closing(object sender, System.ComponentModel.CancelEventArgs e);

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
                return;

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

        public NativeSeparatorItem AddSeparator()
        {
            NativeSeparatorItem nativeSeparatorItem;

            Add(nativeSeparatorItem = new NativeSeparatorItem());

            return nativeSeparatorItem;
        }

        public abstract void Tick();

        public abstract string GetMenuTitle();

        public abstract string GetMenuDescription();

        public abstract string GetItemTitle(string itemName);

        public abstract string GetItemDescription(string itemName);

        public abstract string GetItemValueTitle(string itemName, string valueName);

        public abstract string GetItemValueDescription(string itemName, string valueName);

        public string GetItemValueTitle(object sender, string valueName)
        {
            return GetItemValueTitle(((NativeItem)sender).Tag.ToString(), valueName);
        }

        public string[] GetItemValueTitle(string itemName, params string[] valueNames)
        {
            string[] ret = new string[valueNames.Length];

            for (int i = 0; i < valueNames.Length; i++)
                ret[i] = GetItemValueTitle(itemName, valueNames[i]);

            return ret;
        }

        public string GetItemValueDescription(object sender, string valueName)
        {
            return GetItemValueDescription(((NativeItem)sender).Tag.ToString(), valueName);
        }

        public string[] GetItemValueDescription(string itemName, params string[] valueNames)
        {
            string[] ret = new string[valueNames.Length];

            for (int i = 0; i < valueNames.Length; i++)
                ret[i] = GetItemValueDescription(itemName, valueNames[i]);

            return ret;
        }

        public NativeSubmenuItem NewSubmenu(NativeMenu menu, string menuName)
        {
            NativeSubmenuItem item = AddSubMenu(menu);
            item.Title = GetItemTitle(menuName);
            item.Description = GetItemDescription(menuName);

            item.Tag = menuName;

            return item;
        }

        public NativeCheckboxItem NewCheckboxItem(string itemName)
        {
            NativeCheckboxItem item;

            Add(item = new NativeCheckboxItem(GetItemTitle(itemName), GetItemDescription(itemName)));

            item.Tag = itemName;

            return item;
        }

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

        public NativeItem NewItem(string itemName)
        {
            NativeItem item;

            Add(item = new NativeItem(GetItemTitle(itemName), GetItemDescription(itemName)));

            item.Tag = itemName;

            return item;
        }

        public NativeItem NewItem(int pos, string itemName)
        {
            NativeItem item;

            Add(pos, item = new NativeItem(GetItemTitle(itemName), GetItemDescription(itemName)));

            item.Tag = itemName;

            return item;
        }
    }
}