using GTA;
using Notification = GTA.UI.Notification;
using Screen = GTA.UI.Screen;

namespace FusionLibrary
{
    public interface ICustomTextInterface
    {
        string GetLocalizedText(string entry);

        string[] GetLocalizedText(params string[] entries);

        void ShowSubtitle(string entry, int duration = 2500);

        void ShowSubtitle(string entry, int duration = 2500, params string[] values);

        void ShowHelp(string entry, bool beep = true);

        void ShowHelp(string entry, bool beep, params object[] values);

        void ShowNotification(string entry, bool blinking = false);

        void ShowNotification(string entry, bool blinking, params string[] values);
    }

    public class CustomText : ICustomTextInterface
    {
        public string EntryModel { get; }

        public CustomText(string entryModel)
        {
            EntryModel = entryModel;
        }

        public string GetLocalizedText(string entry)
        {
            return Game.GetLocalizedString($"{EntryModel}_Text_{entry}"); 
        }

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

        public void ShowSubtitle(string entry, int duration = 2500)
        {
            Screen.ShowSubtitle(GetLocalizedText(entry), duration);
        }

        public void ShowSubtitle(string entry, int duration = 2500, params string[] values)
        {
            Screen.ShowSubtitle(string.Format(GetLocalizedText(entry), values), duration);
        }

        public void ShowHelp(string entry, bool beep = true)
        {
            Screen.ShowHelpText(GetLocalizedText(entry), -1, beep);
        }

        public void ShowHelp(string entry, bool beep, params object[] values)
        {
            Screen.ShowHelpText(string.Format(GetLocalizedText(entry), values), -1, beep);
        }

        public void ShowNotification(string entry, bool blinking = false)
        {
            Notification.Show(GetLocalizedText(entry), blinking);
        }

        public void ShowNotification(string entry, bool blinking, params string[] values)
        {
            Notification.Show(string.Format(GetLocalizedText(entry), values), blinking);
        }

        public string GetMenuTitle(string menuName)
        {
            return Game.GetLocalizedString($"{EntryModel}_Menu_{menuName}_Title");
        }

        public string GetMenuDescription(string menuName)
        {
            return Game.GetLocalizedString($"{EntryModel}_Menu_{menuName}_Description");
        }

        public string GetItemTitle(string menuName, string itemName)
        {
            return Game.GetLocalizedString($"{EntryModel}_Menu_{menuName}_Item_{itemName}_Title");
        }

        public string GetItemDescription(string menuName, string itemName)
        {
            return Game.GetLocalizedString($"{EntryModel}_Menu_{menuName}_Item_{itemName}_Description");
        }

        public string GetItemValueTitle(string menuName, string itemName, string valueName)
        {
            return Game.GetLocalizedString($"{EntryModel}_Menu_{menuName}_Item_{itemName}_Value_{valueName}_Title");
        }

        public string GetItemValueDescription(string menuName, string itemName, string valueName)
        {
            return Game.GetLocalizedString($"{EntryModel}_Menu_{menuName}_Item_{itemName}_Value_{valueName}_Description");
        }
    }
}
