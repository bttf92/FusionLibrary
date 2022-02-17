using GTA;
using GTA.Native;
using System;
using System.Linq;

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
        }

        private void Main_Tick(object sender, EventArgs e)
        {
            if (Game.IsLoading)
            {
                return;
            }

            if (FusionUtils.FirstTick)
            {
                // Bypass models check
                IntPtr addr = Game.FindPattern("48 85 C0 0F 84 ? ? ? ? 8B 48 50");

                if (addr != IntPtr.Zero)
                {
                    for (int i = 0; i < 24; i++)
                    {
                        unsafe
                        {
                            byte* val = (byte*)(addr + i);

                            *val = 0x90;
                        }
                    }
                }

                Decorator.Initialize();
                TrafficHandler.Init();
            }

            FusionUtils.AllVehicles = World.GetAllVehicles().ToList();

            AnimatePropsHandler.TickAll();
            AnimateProp.TickAll();
            ParticlePlayerHandler.TickAll();
            ParticlePlayer.TickAll();
            InteractiveController.TickAll();
            TimeHandler.Tick();
            CustomNativeMenu.ObjectPool.Process();
            CustomNativeMenu.TickAll();
            ScreenFlash.Tick();
            PlayerSwitch.Tick();
            NativeInput.TickAll();
            ScreenFade.Tick();

            if (PlayerSwitch.Disable)
            {
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, 19, true);
            }

            if (FusionUtils.HideGUI)
            {
                Function.Call(Hash.HIDE_HUD_AND_RADAR_THIS_FRAME);
            }

            if (FusionUtils.HelpText != null)
            {
                GTA.UI.Screen.ShowHelpTextThisFrame($"{FusionUtils.HelpText}");
                FusionUtils.HelpText = null;
            }

            if (FusionUtils.SubtitleText != null)
            {
                GTA.UI.Screen.ShowSubtitle($"{FusionUtils.SubtitleText}");
                FusionUtils.SubtitleText = null;
            }

            if (FusionUtils.NotificationText != null)
            {
                GTA.UI.Notification.Show($"{FusionUtils.NotificationText}");
                FusionUtils.NotificationText = null;
            }

            if (FusionUtils.FirstTick)
            {
                FusionUtils.FirstTick = false;
            }
        }
    }
}
