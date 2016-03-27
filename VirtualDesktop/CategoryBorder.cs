using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VirtualDesktop
{
    public class CategoryBorder : Border
    {
        public static LinearGradientBrush SelectedBackgroundBrush =
            new LinearGradientBrush(
                    Color.FromRgb(75, 108, 183)
                    , Color.FromRgb(24, 40, 72)
                    , new Point(0.5, 0)
                    , new Point(0.5, 1));

        public static LinearGradientBrush UnselectedBackgroundBrush =
            new LinearGradientBrush(
                    Color.FromRgb(72, 85, 99)
                    , Color.FromRgb(41, 50, 60)
                    , new Point(0, 0)
                    , new Point(0.5, 1));

        private Category category;

        public CategoryBorder() { }

        public CategoryBorder(Category category)
        {
            this.category = category;
        }

        public Category Category
        {
            get
            {
                return category;
            }
            set
            {
                category = value;
            }
        }

        public void Deselect()
        {    
            Background = UnselectedBackgroundBrush;
            category.IsSelected = false;
        }

        public void Select()
        {
            App.MainAppWindow.SliderWindow.SelectedCategoryBorder = this;
            Background = SelectedBackgroundBrush;
            BorderBrush = Brushes.Transparent;
            CategoryManager.SelectedCategoryIndex = category.Index;
            category.IsSelected = true;
        }
    }
}
