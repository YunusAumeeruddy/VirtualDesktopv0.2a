using System;
using System.Threading;
using System.Windows;

// Mutex code : http://stackoverflow.com/questions/28275119/how-can-i-check-if-my-program-is-already-running

namespace VirtualDesktop
{
    public class Startup
    {
        static Mutex mutex = new Mutex(true , GenerateMutexName());

        [STAThread]
        static void Main()
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                try
                {
                    App app = new App();
                    app.Run();
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
            else
            {
                string title = "Application already running";
                string message = "Only 1 instance of this application is allowed to run.";
                message += "\nYou can run another instance by moving a copy of all application files";
                message += " to another directory and then running it from there.";
                MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private static string GenerateMutexName()
        {
            string mutexName = ExternalResourceDirManager.GetAbsolutePathAsOfAppDirectory("App");
            mutexName = mutexName.Replace(":","").Replace(@"\","");
            return mutexName;
        }
    }
}