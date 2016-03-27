using System;
using System.Windows;
using VirtualDesktop.Actions;

namespace VirtualDesktop
{
    /// <summary>
    /// Interaction logic for RenameShortcutIconWindow.xaml
    /// </summary>
    public partial class RenameShortcutIconWindow : Window
    {
        private ShortcutIcon shortcutIcon;

        public RenameShortcutIconWindow(ShortcutIcon shortcutIcon)
        {
            InitializeComponent();
            Owner = OverlayWindow.MainOverlayWindow;
            this.shortcutIcon = shortcutIcon;
            txtName.Text = shortcutIcon.LabelText;
            txtName.Focus();
            txtName.SelectAll();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }

        private void txtName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string currentText = txtName.Text;
            if (currentText == null
                || currentText.Length == 0
                || currentText.Equals(shortcutIcon.LabelText))
                btnOk.IsEnabled = false;
            else
                btnOk.IsEnabled = true;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            string oldName = shortcutIcon.LabelText;
            string newName = txtName.Text;
            shortcutIcon.RenameLabelText(newName);
            bool IsFileRenamed = false;
            if (chkbxRenameFile.IsChecked.Value)
            {
                shortcutIcon.RenameFile(newName);
                IsFileRenamed = true;
            }

            ActionSet actionSet = new ActionSet();
            RenameShortcutIconAction action =
                new RenameShortcutIconAction(shortcutIcon , oldName , newName , IsFileRenamed);
            actionSet.AddAction(action);
            ActionManager.AddNewActionSet(actionSet);
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.MainAppWindow.DefreezeWindow();
        }
    }
}
