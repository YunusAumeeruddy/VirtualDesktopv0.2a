using Microsoft.Win32;
using System;
using System.Windows;

namespace VirtualDesktop
{
    public class ScreenWorkAreaUtils
    {
        private static double workAreaWidth = GetWorkAreaWidth();
        private static double workAreaHeight = GetWorkAreaHeight();

        public static void DetectResolutionChange()
        {
            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
        }

        private static double GetWorkAreaHeight()
        {
            return SystemParameters.WorkArea.Height;
        }

        private static double GetWorkAreaWidth()
        {
            return SystemParameters.WorkArea.Width;
        }

        public static Double ScreenWorkAreaHeight
        {
            get
            {
                return workAreaHeight;
            }
        }

        public static Double ScreenWorkAreaWidth
        {
            get
            {
                return workAreaWidth;
            }
        }

        private static void SystemEvents_DisplaySettingsChanged(object sender , EventArgs e)
        {
            workAreaWidth = GetWorkAreaWidth();
            workAreaHeight = GetWorkAreaHeight();
            // Do resizing and re-positioning of main window
        }
    }
}
