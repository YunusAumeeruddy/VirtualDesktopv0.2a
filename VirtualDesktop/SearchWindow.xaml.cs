using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace VirtualDesktop
{
    /// <summary>
    /// Interaction logic for SearchWindow.xaml
    /// </summary>
    public partial class SearchWindow : Window
    {
        public static SearchWindow CurrentSearchWindow;
        public static SearchWindowSelectCategory CurrentSearchWindowSelectCategory;

        public static List<Category> lstAllCategories;
        public static List<Category> lstSelectedCategory = new List<Category>();

        private int noOfRows = 0;
        private int noOfCols = 0;
        private double extraColumnWidth = 0;
        private double extraRowHeight = 0;

        private double OriginalDisplayCanvasHeight = 0;

        public SearchWindow()
        {
            InitializeComponent();
            Owner = OverlayWindow.MainOverlayWindow;
            CurrentSearchWindow = this;
            OriginalDisplayCanvasHeight = DisplayCanvas.Height;
            lstAllCategories = CategoryManager.CategoryListSortedByName;
            lstSelectedCategory.Add(CategoryManager.GetSelectedCategory());
            RefreshLblSelectedCategoriesText();
            CalculateNoOfCols();
            CalculateNoOfRows();
            CalculateExtraColumnWidth();
            CalculateExtraRowHeight();
        }

        public void AddShortcutIconBorderToDisplayCanvas(ShortcutIconBorder border, int rowNo, int colNo)
        {
            double leftCanvasMargin = colNo * (ColumnManager.CellWidth + extraColumnWidth);
            double topCanvasMargin = rowNo * (RowManager.CellHeight + extraRowHeight);
            Canvas.SetLeft(border, leftCanvasMargin);
            Canvas.SetTop(border, topCanvasMargin);
            DisplayCanvas.Children.Add(border);
        }

        public void CalculateExtraColumnWidth()
        {
            double totalWidthCoveredByCells = noOfCols * ColumnManager.CellWidth;
            double totalExtraSpaceLeft = DisplayCanvas.Width - totalWidthCoveredByCells;
            extraColumnWidth = totalExtraSpaceLeft / (noOfCols - 1);
        }

        public void CalculateExtraRowHeight()
        {
            double totalHeightCoveredByCells = noOfRows * RowManager.CellHeight;
            double totalExtraSpaceLeft = DisplayCanvas.Height - totalHeightCoveredByCells;
            extraRowHeight = totalExtraSpaceLeft / (noOfRows - 1);
        }

        public void CalculateNoOfRows()
        {
            noOfRows = (int)(DisplayCanvas.Height / RowManager.CellHeight);
        }

        public void CalculateNoOfCols()
        {
            noOfCols = (int)(DisplayCanvas.Width / ColumnManager.CellWidth);
        }

        public void DoSearch()
        {
            DisplayCanvas.Children.RemoveRange(0 , DisplayCanvas.Children.Count);
            DisplayCanvas.Height = OriginalDisplayCanvasHeight;
            int rowNo = 0;
            int colNo = 0;

            string searchText = txtSearch.Text;

            List<ShortcutIcon> lstShortcutIcon = new List<ShortcutIcon>();
            foreach (Category category in lstSelectedCategory)
            {
                ShortcutIconGrid shortcutIconGrid = category.ShortcutIconGrid;
                foreach (ShortcutIcon shortcutIcon in shortcutIconGrid.ShortcutIconList)
                {
                    if (shortcutIcon.LabelText.ToLower().Contains(searchText.ToLower()))
                    {
                        lstShortcutIcon.Add(shortcutIcon);
                    }
                }
            }

            string searchStatus = " No shortcut icons found.";
            if(lstShortcutIcon.Count > 0)
            {
                searchStatus = " " + lstShortcutIcon.Count + " shortcut icon";

                if(lstShortcutIcon.Count == 1)
                {
                    searchStatus += " found";
                }
                else
                {
                    searchStatus += "s found";
                }
            }
            txtBSearchStatus.Text = searchStatus;

            lstShortcutIcon = lstShortcutIcon.OrderBy(o => o.LabelText).ToList();
            foreach (ShortcutIcon shortcutIcon in lstShortcutIcon)
            {
                ShortcutIconBorder shortcutIconBorder = 
                    ShortcutIconBorder.GenerateShortcutIconBorderWOEvent(shortcutIcon);

                shortcutIconBorder.TextBlock.Foreground = Brushes.White;
                shortcutIconBorder.TextBlock.UnderlineText(searchText);

                AddShortcutIconBorderToDisplayCanvas(shortcutIconBorder , rowNo , colNo);

                colNo++;
                if (colNo == noOfCols)
                {
                    colNo = 0;
                    rowNo++;
                    if(rowNo >= noOfRows)
                    {
                        DisplayCanvas.Height += RowManager.CellHeight + extraRowHeight;
                    }
                }

            }
        }

        public void RefreshBtnSearch()
        {
            bool enabled = false;

            if (txtSearch.Text != null
                && txtSearch.Text.Length > 0
                && lstSelectedCategory.Count > 0)
            {
                enabled = true;
            }
            btnSearch.IsEnabled = enabled;
        }

        public void RefreshLblSelectedCategoriesText()
        {
            StringBuilder stringBuilder = new StringBuilder();
            lblSelectedCategories.Foreground = Brushes.White;
            if (lstSelectedCategory.Count == 0)
            {
                stringBuilder.Append("None selected");
                lblSelectedCategories.Foreground = Brushes.Red;
            }
            else
            {
                if (lstSelectedCategory.Count == CategoryManager.CategoryCount)
                {
                    stringBuilder.Append("All ");
                }
                else
                {
                    stringBuilder.Append(lstSelectedCategory.Count + " categor");
                    stringBuilder.Append(lstSelectedCategory.Count == 1 ? "y" : "ies");
                }

                stringBuilder.Append(" selected (");
                for(int i = 0; i < lstSelectedCategory.Count; i++)
                {
                    if(lstSelectedCategory[i].Name != null)
                        stringBuilder.Append(lstSelectedCategory[i].Name);
                    if (i < lstSelectedCategory.Count - 1)
                    {
                        stringBuilder.Append(" , ");
                    } 
                }
                stringBuilder.Append(")");
            }
            lblSelectedCategories.Content = stringBuilder.ToString();
            ToolTip tooltip = new ToolTip();
            tooltip.Content = stringBuilder.ToString();
            lblSelectedCategories.ToolTip = tooltip;
        }

        public void SelectAllCategories()
        {
            lstSelectedCategory.Clear();
            foreach (Category category in lstAllCategories)
            {
                lstSelectedCategory.Add(category); 
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.MainAppWindow.DefreezeWindow();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshBtnSearch();
        }

        private void btnSelectCategory_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            if (CurrentSearchWindowSelectCategory == null)
            {
                CurrentSearchWindowSelectCategory = new SearchWindowSelectCategory(this);
            }
            CurrentSearchWindowSelectCategory.Show();
            CurrentSearchWindowSelectCategory.lstBxCategories.Focus();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            DoSearch();
        }

        private void txtSearch_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == Key.Enter && btnSearch.IsEnabled)
            {
                DoSearch();
            }
        }
    }
}
