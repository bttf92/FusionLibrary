using GTA;
using Notification = GTA.UI.Notification;
using Screen = GTA.UI.Screen;

namespace FusionLibrary
{
    /// <summary>
    /// Helper for custom localized strings.
    /// </summary>
    public class CustomText
    {
        /// <summary>
        /// Base model for localized text.
        /// </summary>
        public string EntryModel { get; }

        /// <summary>
        /// Instaces a new <see cref="CustomText"/> helper using <paramref name="entryModel"/>.
        /// </summary>
        /// <param name="entryModel">Base model for localized text.</param>
        public CustomText(string entryModel)
        {
            EntryModel = entryModel;
        }

        /// <summary>
        /// Returns the localized text using this format: <see langword="{EntryModel}_Text_{entry}"/>.
        /// </summary>
        /// <param name="entry">Entry name.</param>
        /// <returns>Localized text.</returns>
        public string GetLocalizedText(string entry)
        {
            return Game.GetLocalizedString($"{EntryModel}_Text_{entry}"); 
        }

        /// <summary>
        /// Returns a list of localized texts using <paramref name="entries"/> as names.
        /// </summary>
        /// <param name="entries">List of entries.</param>
        /// <returns>Localized texts.</returns>
        public string[] GetLocalizedText(params string[] entries)
        {
            string[] ret = new string[entries.Length];

            for (int i = 0; i < entries.Length; i++)
            {
                ret[i] = GetLocalizedText(entries[i]);
            }

            return ret;
        }

        public string GetOnOff(bool value)
        {
            return value ? GetLocalizedText("On") : GetLocalizedText("Off");
        }

        /// <summary>
        /// Shows a localized subtitle at the bottom of the screen for a given time
        /// </summary>
        /// <param name="entry">Name of the entry.</param>
        /// <param name="duration">The duration to display the subtitle in milliseconds.</param>
        public void ShowSubtitle(string entry, int duration = 2500)
        {
            Screen.ShowSubtitle(GetLocalizedText(entry), duration);
        }

        public void ShowSubtitle(string entry, params object[] values)
        {
            Screen.ShowSubtitle(string.Format(GetLocalizedText(entry), values));
        }

        public void ShowSubtitle(string entry, int duration = 2500, params object[] values)
        {
            Screen.ShowSubtitle(string.Format(GetLocalizedText(entry), values), duration);
        }

        /// <summary>
        /// Displays a localized help message in the top corner of the screen infinitely.
        /// </summary>
        /// <param name="entry">Name of the entry.</param>
        /// <param name="duration">
        /// The duration how long the help text will be displayed in real time (not in game time which is influenced by game speed).
        /// if the value is not positive, the help text will be displayed for 7.5 seconds.
        /// </param>
        /// <param name="beep">Whether to play beeping sound.</param>
        /// <param name="looped">Whether to show this help message forever.</param>
        public void ShowHelp(string entry, int duration = -1, bool beep = true, bool looped = false)
        {
            Screen.ShowHelpText(GetLocalizedText(entry), duration, beep, looped);
        }

        public void ShowHelp(string entry, params object[] values)
        {
            Screen.ShowHelpText(string.Format(GetLocalizedText(entry), values));
        }

        public void ShowHelp(string entry, bool beep, params object[] values)
        {
            Screen.ShowHelpText(string.Format(GetLocalizedText(entry), values), -1, beep);
        }

        public void ShowHelp(string entry, int duration = -1, bool beep = true, bool looped = false, params object[] values)
        {
            Screen.ShowHelpText(string.Format(GetLocalizedText(entry), values), duration, beep, looped);
        }

        /// <summary>
        /// Creates a localized <see cref="Notification"/> above the minimap with the given message.
        /// </summary>
        /// <param name="entry">Name of the entry.</param>
        /// <param name="blinking">if set to <see langword="true" /> the notification will blink.</param>
        /// <returns>The handle of the <see cref="Notification"/> which can be used to hide it using <see cref="Notification.Hide(int)"/>.</returns>
        public void ShowNotification(string entry, bool blinking = false)
        {
            Notification.Show(GetLocalizedText(entry), blinking);
        }

        public void ShowNotification(string entry, bool blinking = false, params string[] values)
        {
            Notification.Show(string.Format(GetLocalizedText(entry), values), blinking);
        }

        /// <summary>
        /// Returns the localized menu title.
        /// </summary>
        /// <param name="menuName">Name of the menu.</param>
        /// <returns>Localized menu title.</returns>
        public string GetMenuTitle(string menuName)
        {
            return Game.GetLocalizedString($"{EntryModel}_Menu_{menuName}_Title");
        }

        /// <summary>
        /// Returns the localized menu description.
        /// </summary>
        /// <param name="menuName">Name of the menu.</param>
        /// <returns>Localized menu description.</returns>
        public string GetMenuDescription(string menuName)
        {
            return Game.GetLocalizedString($"{EntryModel}_Menu_{menuName}_Description");
        }

        /// <summary>
        /// Returns the localized item tile of the specified menu.
        /// </summary>
        /// <param name="menuName">Name of the menu.</param>
        /// <param name="itemName">Name of the item.</param>
        /// <returns>Localized item title.</returns>
        public string GetItemTitle(string menuName, string itemName)
        {
            return Game.GetLocalizedString($"{EntryModel}_Menu_{menuName}_Item_{itemName}_Title");
        }

        /// <summary>
        /// Returns the localized item description of the specified menu.
        /// </summary>
        /// <param name="menuName">Name of the menu.</param>
        /// <param name="itemName">Name of the item.</param>
        /// <returns>Localized item description.</returns>
        public string GetItemDescription(string menuName, string itemName)
        {
            return Game.GetLocalizedString($"{EntryModel}_Menu_{menuName}_Item_{itemName}_Description");
        }

        /// <summary>
        /// Returns the localized value title of an item of the specified menu.
        /// </summary>
        /// <param name="menuName">Name of the menu.</param>
        /// <param name="itemName">Name of the item.</param>
        /// <param name="valueName">Name of the value.</param>
        /// <returns>Localized value title.</returns>
        public string GetItemValueTitle(string menuName, string itemName, string valueName)
        {
            return Game.GetLocalizedString($"{EntryModel}_Menu_{menuName}_Item_{itemName}_Value_{valueName}_Title");
        }

        /// <summary>
        /// Returns the localized value description of an item of the specified menu.
        /// </summary>
        /// <param name="menuName">Name of the menu.</param>
        /// <param name="itemName">Name of the item.</param>
        /// <param name="valueName">Name of the value.</param>
        /// <returns>Localized value description.</returns>
        public string GetItemValueDescription(string menuName, string itemName, string valueName)
        {
            return Game.GetLocalizedString($"{EntryModel}_Menu_{menuName}_Item_{itemName}_Value_{valueName}_Description");
        }
    }
}
