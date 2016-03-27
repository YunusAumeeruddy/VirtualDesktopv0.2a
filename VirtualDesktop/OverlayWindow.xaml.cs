using System.Windows;

namespace VirtualDesktop
{
    /// <summary>
    /// Interaction logic for OverlayWindow.xaml
    /// </summary>
    public partial class OverlayWindow : Window
    {
        public static OverlayWindow MainOverlayWindow;

        public OverlayWindow()
        {
            InitializeComponent();
            Owner = App.MainAppWindow.SliderWindow;
            MainOverlayWindow = this;
            Top = Owner.Top;
            Left = Owner.Left;
            Width = ScreenWorkAreaUtils.ScreenWorkAreaWidth;
            Height = ScreenWorkAreaUtils.ScreenWorkAreaHeight;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Owner.Show();
        }
    }
}
