using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace VirtualDesktop
{
    /// <summary>
    /// Interaction logic for ManageCategoryFileExt.xaml
    /// </summary>
    public partial class ManageCategoryFileExt : Window
    {
        public string CategoryName;
        private List<string> lstFileExts;

        private bool changes = false;

        public ManageCategoryFileExt(string categoryName)
        {
            InitializeComponent();
            Owner = OverlayWindow.MainOverlayWindow;
            CategoryName = categoryName;
            lblTitle.Content += "\"" + CategoryName + "\"";
            LoadFileExts();
        }

        private void AddFileExt()
        {
            btnAddExt.IsEnabled = false;
            string fileExt = txtFileExt.Text;
            lstBxFileExt.Items.Add(fileExt);
            lstBxFileExt.Items.SortDescriptions.Add(
                new System.ComponentModel.SortDescription("",
                    System.ComponentModel.ListSortDirection.Ascending));

            txtFileExt.Text = "";
            txtFileExt.Focus();
            lblStatus.Foreground = Brushes.White;
            lblStatus.Content = "File extension \"" + fileExt + "\" successfully added to the list!";

            lstFileExts.Add(fileExt);
            lstFileExts.Sort();
            changes = true;
            btnSave.IsEnabled = true;
        }

        private void LoadFileExts()
        {
            lstFileExts = App.MainFileExtCategory.GetFileExtListForCategory(CategoryName);
            lstFileExts.ForEach(item => lstBxFileExt.Items.Add(item));
        }

        private void SaveChanges()
        {
            if (changes)
            {
                changes = false;
                btnSave.IsEnabled = false;
                App.MainFileExtCategory.ModifyCategoryFileExtensions(CategoryName , lstFileExts);
            }
        }

        // EVENTS

        private void btnAddExt_Click(object sender, RoutedEventArgs e)
        {
            AddFileExt();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (changes)
            {
                string title = "Unsaved changes";
                string msg = "You have unsaved changes. Would you like to save those changes before closing this window?";
                MessageBoxResult res = MessageBox.Show(msg , title , MessageBoxButton.YesNo , MessageBoxImage.Question);
                if(res == MessageBoxResult.Yes)
                {
                    SaveChanges();
                }
            }
            Close();
        }

        private void btnRemoveExt_Click(object sender, RoutedEventArgs e)
        {
            while (true)
            {
                int selectedIndex = lstBxFileExt.SelectedIndex;
                if (selectedIndex == -1)
                {
                    break;
                }
                else
                {
                    lstBxFileExt.Items.RemoveAt(selectedIndex);
                    lstFileExts.RemoveAt(selectedIndex);
                    changes = true;
                    btnSave.IsEnabled = true;
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveChanges();
            lblStatus.Foreground = Brushes.White;
            lblStatus.Content = "Save successful!";
        }

        private bool HasFileExtension(string fileExt)
        {
            foreach (string ext in lstFileExts)
            {
                if (fileExt.ToLower().Equals(ext.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }

        private void lstBxFileExt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstBxFileExt.SelectedItems.Count == 0)
            {
                btnRemoveExt.IsEnabled = false;
            }
            else
            {
                btnRemoveExt.IsEnabled = true;
            }
        }

        private void txtFileExt_KeyDown(object sender , KeyEventArgs e)
        {
            if(e.Key == Key.Enter && btnAddExt.IsEnabled)
            {
                AddFileExt();
            }
        }

        private void txtFileExt_TextChanged(object sender, TextChangedEventArgs e)
        {
            string fileExtEntered = txtFileExt.Text;
            bool btnAddEnabled = false;
            Brush foregroundBrush = Brushes.PaleVioletRed;
            string status = "";
            if(fileExtEntered == null || fileExtEntered.Length == 0)
            {
                status = "No file extension entered!";
            }
            else if (HasFileExtension(fileExtEntered))
            {
                status = "This category already has the file extension '" 
                       + fileExtEntered
                       + "' in the list.";
            }
            else if(App.MainFileExtCategory.ContainsFileExtension(fileExtEntered))
            {
                string categoryName = App.MainFileExtCategory.GetFileExtCategoryName(fileExtEntered);
                status = "File extension is already attached to '" + categoryName +"' category.";
            }
            else
            {
                status = "File extension '" + fileExtEntered + "' can be added!";
                btnAddEnabled = true;
                foregroundBrush = Brushes.White;
            }
            btnAddExt.IsEnabled = btnAddEnabled;
            lblStatus.Foreground = foregroundBrush;
            lblStatus.Content = status;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.MainAppWindow.DefreezeWindow();
        }

    }
}
