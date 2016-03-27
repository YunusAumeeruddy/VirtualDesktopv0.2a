using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using VirtualDesktop.Actions;

namespace VirtualDesktop
{
    /// <summary>
    /// Interaction logic for NewShortcutIconWindow.xaml
    /// </summary>
    public partial class NewShortcutIconWindow : Window
    {
        public static int selectedOptionIndex = 0;

        public NewShortcutIconWindow()
        {
            InitializeComponent();
            Owner = OverlayWindow.MainOverlayWindow;
            if(selectedOptionIndex == 0)
            {
                rdoFile.IsChecked = true;
            }
            else if(selectedOptionIndex == 1)
            {
                rdoFolder1.IsChecked = true;
            }
            else
            {
                rdoFolder2.IsChecked = true;
            }
        }

        private void btnSelectFilesFolders_Click(object sender, RoutedEventArgs e)
        {
            List<string> lstFilePath = new List<string>();

            CommonOpenFileDialog openFileDialog = new CommonOpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.NavigateToShortcut = false;
            if (rdoFile.IsChecked.Value)
            {
                openFileDialog.IsFolderPicker = false;
            }
            else
            {
                openFileDialog.IsFolderPicker = true;
            }

            CommonFileDialogResult result = openFileDialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                IEnumerable<string> fileNames = openFileDialog.FileNames;
                
                foreach (string fileName in fileNames)
                {
                    if (rdoFile.IsChecked.Value || rdoFolder1.IsChecked.Value)
                    {
                        lstFilePath.Add(fileName);
                    }
                    else
                    {
                        string[] filePaths = Directory.GetFiles(@fileName);
                        foreach (string filePath in filePaths)
                        {
                            lstFilePath.Add(filePath);
                        }
                    }
                }
            }

            foreach (string filePath in lstFilePath)
            {
                ShortcutIcon shortcutIcon = ShortcutIcon.GenerateShortcutIconFromFile(filePath , null);
                int categoryIndex = shortcutIcon.Category.Index;
                ShortcutIconGrid shortcutIconGrid = CategoryManager.GetShortcutIconGrid(categoryIndex);
                shortcutIconGrid.LoadShortcutIcon(shortcutIcon);
            }

            if (ConfigManager.AutoSortMode)
            {
                App.MainAppWindow.SortAllCategoryShortcutIcons();
            }
            else
            {
                App.MainAppWindow.PopulateSelectedShortcutIconGrid();

            }
            App.MainAppWindow.SliderWindow.RefreshTopMenuShowPanelLabels();
            Close();
            e.Handled = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.MainAppWindow.DefreezeWindow();
        }

        private void rdoFile_Checked(object sender, RoutedEventArgs e)
        {
            selectedOptionIndex = 0;
        }

        private void rdoFolder1_Checked(object sender, RoutedEventArgs e)
        {
            selectedOptionIndex = 1;
        }

        private void rdoFolder2_Checked(object sender, RoutedEventArgs e)
        {
            selectedOptionIndex = 2;
        }
    }
}
