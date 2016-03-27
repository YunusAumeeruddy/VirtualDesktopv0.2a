using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VirtualDesktop
{
    /// <summary>
    /// Interaction logic for SearchWindowSelectCategory.xaml
    /// </summary>
    public partial class SearchWindowSelectCategory : Window
    {
        public SearchWindow SearchWindowParent;
        private bool IsItemsLoaded = false;

        public SearchWindowSelectCategory(SearchWindow searchWindow)
        {
            InitializeComponent();
            Owner = searchWindow;
            SearchWindowParent = searchWindow;
            LoadCategories();
        }

        public void LoadCategories()
        {
            lstBxCategories.ItemsSource = SearchWindow.lstAllCategories;
            foreach (Category category in lstBxCategories.Items)
            {
                if (SearchWindow.lstSelectedCategory.Contains(category))
                {
                    lstBxCategories.SelectedItems.Add(category);
                }
            }
        }

        private void lstBxCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsItemsLoaded)
            {
                IsItemsLoaded = true;
                return;
            }

            SearchWindow.lstSelectedCategory.Clear();
            List<Category> selectedCategories = new List<Category>();
            foreach (Category category in lstBxCategories.SelectedItems)
            {
                selectedCategories.Add(category);
            }

            selectedCategories = selectedCategories.OrderBy(o => o.Name).ToList();

            foreach (Category category in selectedCategories)
            {
                SearchWindow.lstSelectedCategory.Add(category);
            }

            SearchWindowParent.RefreshLblSelectedCategoriesText();
            SearchWindowParent.RefreshBtnSearch();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            SearchWindowParent.IsEnabled = true;
        }

        private void btnDeselectAll_Click(object sender, RoutedEventArgs e)
        {
            lstBxCategories.SelectedItems.Clear();
            SearchWindow.lstSelectedCategory.Clear();
            SearchWindowParent.RefreshLblSelectedCategoriesText();
            SearchWindowParent.RefreshBtnSearch();
        }

        private void btnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            lstBxCategories.SelectedItems.Clear();
            foreach (Category category in lstBxCategories.Items)
            {
                lstBxCategories.SelectedItems.Add(category);
            }
            lstBxCategories.Focus();
            SearchWindowParent.SelectAllCategories();
            SearchWindowParent.RefreshLblSelectedCategoriesText();
            SearchWindowParent.RefreshBtnSearch();
        }
    }
}
