using GTA;
using Screen = GTA.UI.Screen;
using Notification = GTA.UI.Notification;

namespace FusionLibrary
{
    public interface CustomTextInterface
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

    public class CustomText : CustomTextInterface
    {        
        public string EntryModel { get; }

        public CustomText(string entryModel)
        {
            EntryModel = entryModel;
        }

        public string GetLocalizedText(string entry)
        {
            return Game.GetLocalizedString(string.Format(EntryModel, entry));
        }

        public string[] GetLocalizedText(params string[] entries)
        {
            string[] ret = new string[entries.Length];

            for (int i = 0; i < entries.Length; i++)
                ret[i] = GetLocalizedText(entries[i]);

            return ret;
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
            Screen.ShowHelpTextThisFrame(GetLocalizedText(entry), beep);
        }

        public void ShowHelp(string entry, bool beep, params object[] values)
        {
            Screen.ShowHelpTextThisFrame(string.Format(GetLocalizedText(entry), values), beep);
        }

        public void ShowNotification(string entry, bool blinking = false)
        {
            Notification.Show(GetLocalizedText(entry), blinking);
        }

        public void ShowNotification(string entry, bool blinking, params string[] values)
        {
            Notification.Show(string.Format(GetLocalizedText(entry), values), blinking);
        }
    }
}
