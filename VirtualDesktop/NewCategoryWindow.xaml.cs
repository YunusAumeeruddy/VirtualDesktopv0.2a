using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VirtualDesktop.Actions;

namespace VirtualDesktop
{
    /// <summary>
    /// Interaction logic for NewCategoryWindow.xaml
    /// </summary>
    public partial class NewCategoryWindow : Window
    {
        private ImageSource noImageSource = null;
        private string selectedImagePath = null;

        public NewCategoryWindow()
        {
            InitializeComponent();
            Owner = OverlayWindow.MainOverlayWindow;
            noImageSource = imgCategory.Source;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }

        private void btnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image Files|*.bmp;*.gif;*.ico;*.jpg;*.png;*.wdp;*.tiff";
            dialog.Multiselect = false;
            DialogResult result = dialog.ShowDialog();
            if(result == System.Windows.Forms.DialogResult.OK)
            {
                selectedImagePath = dialog.FileName;
                BitmapImage imageSource = new BitmapImage();
                imageSource.BeginInit();
                imageSource.UriSource = new Uri(selectedImagePath);
                imageSource.EndInit();
                imgCategory.Source = imageSource;
                imgCategory.ToolTip = new System.Windows.Controls.ToolTip()
                {
                    Content = selectedImagePath
                };
                btnClearImage.IsEnabled = true;
            }
        }

        private void btnClearImage_Click(object sender, RoutedEventArgs e)
        {
            btnClearImage.IsEnabled = false;
            imgCategory.Source = noImageSource;
            imgCategory.ToolTip = new System.Windows.Controls.ToolTip()
            {
                Content = "No image selected"
            };
            selectedImagePath = null;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            string categoryName = txtName.Text;
            bool mainType = chkbxMainType.IsChecked.Value;
            bool mixedType = chkbxMixedType.IsChecked.Value;
            CategoryManager.AddCategory(categoryName , selectedImagePath, mainType , mixedType , null);
            Category category = CategoryManager.LatestCategoryAdded;
            AddCategoryAction action = new AddCategoryAction(category);
            ActionSet actionSet = new ActionSet();
            actionSet.AddAction(action);
            ActionManager.AddNewActionSet(actionSet);
            Close();
        }

        private void txtName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string nameEntered = txtName.Text;
            string txtBNameStatusMsg = "";
            bool btnAddEnabled = false;
            Brush foregroundBrush = Brushes.PaleVioletRed;
            if (nameEntered == null
                || nameEntered.Length == 0)
            {
                txtBNameStatusMsg = "No name entered";
            }
            else if (CategoryManager.HasExistingCategoryName(nameEntered))
            {
                txtBNameStatusMsg = "This category name is in use";
            }
            else
            {
                btnAddEnabled = true;
                txtBNameStatusMsg = "Valid category name!";
                foregroundBrush = Brushes.Green;
            }

            btnAdd.IsEnabled = btnAddEnabled;
            txtBNameStatus.Text = txtBNameStatusMsg;
            txtBNameStatus.Foreground = foregroundBrush;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.MainAppWindow.DefreezeWindow();
        }

        private void chkbxMainType_Checked(object sender, RoutedEventArgs e)
        {
            chkbxMixedType.IsChecked = true;
        }
    }
}
