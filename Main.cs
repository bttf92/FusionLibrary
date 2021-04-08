using GTA;
using GTA.Native;
using System;

namespace FusionLibrary
{
    internal class Main : Script
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

        }

        private void Main_Tick(object sender, EventArgs e)
        {
            if (Game.IsLoading)
                return;

            AnimatePropsHandler.TickAll();
            AnimateProp.TickAll();
            TimeHandler.Tick();
            CustomNativeMenu.ObjectPool.Process();
            CustomNativeMenu.TickAll();
            ScreenFlash.Tick();
            PlayerSwitch.Tick();
            NativeInput.TickAll();

            if (PlayerSwitch.Disable)
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, 19, true);

            if (FusionUtils.HideGUI)
                Function.Call(Hash.HIDE_HUD_AND_RADAR_THIS_FRAME);
        }
    }
}
