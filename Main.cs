using System;
using GTA;
using GTA.Native;

namespace FusionLibrary
{
    public class Main : Script
    {
        public static Version Version => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

        public Main()
        {
            DateTime buildDate = new DateTime(2000, 1, 1).AddDays(Version.Build).AddSeconds(Version.Revision * 2);

            System.IO.File.AppendAllText($"./ScriptHookVDotNet.log", $"FusionLibrary - {Version} ({buildDate})" + Environment.NewLine);

            Tick += Main_Tick;
            Aborted += Main_Aborted;
        }

        private void Main_Aborted(object sender, EventArgs e)
        {
            AnimatePropsHandler.Abort();
        }

        private void Main_Tick(object sender, EventArgs e)
        {
            if (Game.IsLoading)
                return;

            TimeHandler.Process();
            AnimatePropsHandler.ProcessAll();
            CustomNativeMenu.ObjectPool.Process();
            CustomNativeMenu.ProcessAll();
            ScreenFlash.Process();
            PlayerSwitch.Process();
            NativeInput.ProcessAll();            

            if (PlayerSwitch.Disable)
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, 19, true);

            if (Utils.HideGUI)
                Function.Call(Hash.HIDE_HUD_AND_RADAR_THIS_FRAME);
        }
    }
}
