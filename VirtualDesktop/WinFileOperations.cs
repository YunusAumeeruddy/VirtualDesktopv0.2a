using System;
using System.Threading;

namespace VirtualDesktop
{
    public class WinFileOperations
    {
        public static void OpenFile(string filePath)
        {
            try
            {
                Thread thread = new Thread(() => System.Diagnostics.Process.Start(@filePath));
                thread.Start();
            }
            catch (Exception e) { }
        }
    }
}
