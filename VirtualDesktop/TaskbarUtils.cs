using System;
using System.Runtime.InteropServices;

namespace VirtualDesktop
{
    public class TaskbarUtils
    {
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        public static RECT rct;
        public static int TaskbarLeftPos { get; set; } = 0;
        public static int TaskbarTopPos { get; set; } = 0;
        public static int TaskBarWidth { get; set; } = 0;
        public static int TaskBarHeight { get; set; } = 0;

        public static bool TopArea { get; set; } = false;
        public static bool BottomArea { get; set; } = false;
        public static bool LeftArea { get; set; } = false;
        public static bool RightArea { get; set; } = false;

        public static void StoreTaskBarInfo()
        {
            //Get the handle of the task bar
            IntPtr TaskBarHandle;
            TaskBarHandle = FindWindow("Shell_traywnd", "");

            //Get the taskbar window rect in screen coordinates
            GetWindowRect(TaskBarHandle , out rct);

            TaskbarLeftPos = rct.Left;
            TaskbarTopPos = rct.Top;
            TaskBarWidth = rct.Right - rct.Left;
            TaskBarHeight = rct.Bottom - rct.Top;

            BottomArea = false;
            RightArea = false;
            LeftArea = false;
            RightArea = false;

            if (TaskbarLeftPos == 0)
            {
                if (TaskbarTopPos == 0)
                {
                    if (TaskBarWidth == ScreenWorkAreaUtils.ScreenWorkAreaWidth)
                    {
                        TopArea = true;
                    }
                    else
                    {
                        LeftArea = true;
                    }
                }
                else
                {
                    BottomArea = true;
                }
            }
            else
            {
                RightArea = true;
            }
        }
    }
}
