using GTA;
using GTA.Native;
using System;
using System.Linq;
using System.Runtime.InteropServices;

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
                // Bypass model checks
                IntPtr addr = Game.FindPattern("74 12 48 8B 10 48 8B C8 FF 52 30 84 C0 74 05 48 8B C3");
                addr = addr != IntPtr.Zero ? (addr + 11) : IntPtr.Zero;

                unsafe
                {
                    if (addr != IntPtr.Zero && *(byte*)addr != 0x90)
                    {
                        const int bytesToWriteInstructions = 4;
                        byte[] nopBytes = Enumerable.Repeat((byte)0x90, bytesToWriteInstructions).ToArray();
                        Marshal.Copy(nopBytes, 0, addr, bytesToWriteInstructions);
                    }
                }

                // Bypass prop checks
                addr = Game.FindPattern("40 84 ?? 74 13 E8 ?? ?? ?? ?? 48 85 C0 75 09 38 45 57 0F 84");

                unsafe
                {
                    if (addr != IntPtr.Zero)
                    {
                        addr = Game.FindPattern("33 C1 48 8D 4D 6F", new IntPtr((byte*)addr + 0x3A));
                        addr = addr != IntPtr.Zero ? (addr + 0x16) : IntPtr.Zero;

                        if (addr != IntPtr.Zero && *(byte*)addr != 0x90)
                        {
                            const int bytesToWriteInstructions = 0x18;
                            byte[] nopBytes = Enumerable.Repeat((byte)0x90, bytesToWriteInstructions).ToArray();
                            Marshal.Copy(nopBytes, 0, addr, bytesToWriteInstructions);
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
            CustomCameraHandler.TickAll();
            InteractiveController.TickAll();
            CustomNativeMenu.ObjectPool.Process();
            CustomNativeMenu.TickAll();
            ScreenFlash.Tick();
            PlayerSwitch.Tick();
            NativeInput.TickAll();
            ScreenFade.Tick();

            if (PlayerSwitch.Disable)
            {
                Game.DisableControlThisFrame(Control.CharacterWheel);
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
