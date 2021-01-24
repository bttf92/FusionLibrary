using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace FusionLibrary
{
    public static class GameScreen
    {
        private static IntPtr handle;

        static GameScreen()
        {
            handle = User32.FindWindow(null, "Grand Theft Auto V");
        }

        public static Image Capture()
        {
            IntPtr windowDc = User32.GetWindowDC(handle);

            User32.RECT rect = new User32.RECT();
            User32.GetWindowRect(handle, ref rect);

            int nWidth = rect.right - rect.left;
            int nHeight = rect.bottom - rect.top;

            IntPtr compatibleDc = GDI32.CreateCompatibleDC(windowDc);
            IntPtr compatibleBitmap = GDI32.CreateCompatibleBitmap(windowDc, nWidth, nHeight);
            IntPtr hObject = GDI32.SelectObject(compatibleDc, compatibleBitmap);

            GDI32.BitBlt(compatibleDc, 0, 0, nWidth, nHeight, windowDc, 0, 0, GDI32.SRCCOPY);
            GDI32.SelectObject(compatibleDc, hObject);

            Image image = Image.FromHbitmap(compatibleBitmap);

            GDI32.DeleteObject(compatibleBitmap);
            GDI32.DeleteDC(compatibleDc);
            User32.ReleaseDC(handle, windowDc);

            return image;
        }

        public static Image Capture(int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            Image image = Capture();

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public static Stream CaptureToStream(ImageFormat imageFormat)
        {
            return Capture().ToStream(imageFormat);
        }

        public static Stream CaptureToStream(ImageFormat imageFormat, int width, int height)
        {
            return Capture(width, height).ToStream(imageFormat);
        }

        private static Stream ToStream(this Image image, ImageFormat format)
        {
            var stream = new MemoryStream();
            image.Save(stream, format);
            stream.Position = 0;
            return stream;
        }

        private class GDI32
        {
            public const int SRCCOPY = 13369376;

            [DllImport("gdi32.dll")]
            public static extern bool BitBlt(
              IntPtr hObject,
              int nXDest,
              int nYDest,
              int nWidth,
              int nHeight,
              IntPtr hObjectSource,
              int nXSrc,
              int nYSrc,
              int dwRop);

            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

            [DllImport("gdi32.dll")]
            public static extern bool DeleteDC(IntPtr hDC);

            [DllImport("gdi32.dll")]
            public static extern IntPtr DeleteObject(IntPtr hObject);

            [DllImport("gdi32.dll")]
            public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        }

        private class User32
        {
            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();

            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr hWnd);

            [DllImport("user32.dll")]
            public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref GameScreen.User32.RECT rect);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

            public struct RECT
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }
        }
    }
}
