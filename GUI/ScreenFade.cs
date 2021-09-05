using GTA;
using GTA.UI;

namespace FusionLibrary
{
    public static class ScreenFade
    {
        private static int gameTime = 0;
        private static int _fadeInTime;

        public static void FadeOut(int fadeOutTime, int fadeInTime, int waitTime)
        {
            Screen.FadeOut(fadeOutTime);

            _fadeInTime = fadeInTime;
            gameTime = Game.GameTime + waitTime + fadeOutTime;
        }

        internal static void Tick()
        {
            if (gameTime == 0 || Game.GameTime < gameTime)
            {
                return;
            }

            Screen.FadeIn(_fadeInTime);
            gameTime = 0;
        }
    }
}
