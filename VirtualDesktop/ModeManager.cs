using System.Collections.Generic;

namespace VirtualDesktop
{
    public class ModeManager
    {
        private static List<string> modeList = SetModeList();

        public static bool IsModeValid(string mode)
        {
            return modeList.Contains(mode.ToLower());
        }

        private static List<string> SetModeList()
        {
            List<string> modeList = new List<string>();
            modeList.Add("Auto-Categorised".ToLower());
            modeList.Add("Auto-Sort".ToLower());
            return modeList;
        }
    }
}
