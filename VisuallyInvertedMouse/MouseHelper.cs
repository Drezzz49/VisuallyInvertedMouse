using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace VisuallyInvertedMouse
{
    public static class MouseHelper
    {
        // Import the mouse_event function from user32.dll
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        // Mouse actions constants
        private const uint MOUSEEVENTF_LEFTDOWN = 0x02;
        private const uint MOUSEEVENTF_LEFTUP = 0x04;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const uint MOUSEEVENTF_RIGHTUP = 0x10;

        /// <summary>
        /// Simulates a full left mouse click (down then up) at the current cursor position.
        /// </summary>
        public static void SendLeftClick(int delayMs = 5)
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            Thread.Sleep(delayMs); // Short delay to ensure the click is registered
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        /// <summary>
        /// Simulates a full right mouse click at the current cursor position.
        /// </summary>
        public static void SendRightClick(int delayMs = 5)
        {
            mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
            Thread.Sleep(delayMs); // Short delay to ensure the click is registered
            mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
        }

        public static void SetMousePosition(int x, int y)
        {
            // Move the mouse to the specified coordinates
            System.Windows.Forms.Cursor.Position = new System.Drawing.Point(x, y);
        }

    }
}