using System.Windows;

namespace VirtualDesktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static MainWindow MainAppWindow = null;
        public static FileExtCategory MainFileExtCategory = null;

        public App()
        {
            ConsoleManager.ShowMessage("---Testing---");
            MainFileExtCategory = new FileExtCategory();
            MainAppWindow = new MainWindow();
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            MainAppWindow.PrepareShortcutIconGrid();
            TaskbarUtils.StoreTaskBarInfo();
            MainAppWindow.SetWinPosition();
            ConfigManager.LoadConfigFile();
            MainAppWindow.CreateNotifyIcon();
            MainAppWindow.SliderWindow.RefreshTopMenuShowPanelLabels();
            MainAppWindow.Show();
            MainAppWindow.SliderWindow.Owner = MainAppWindow;
            MainAppWindow.SliderWindow.Show();
            //new CheckFreePos().Show();
        }

        public static void ExitApp()
        {
            Current.Shutdown();
        }
    }
}
