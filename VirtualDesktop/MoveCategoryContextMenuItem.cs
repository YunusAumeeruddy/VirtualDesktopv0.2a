using System.Windows;
using System.Windows.Controls;

namespace VirtualDesktop
{
    public class MoveCategoryContextMenuItem : MenuItem
    {
        public Category Category { get; set; }
        private bool moveAction = true;

        public MoveCategoryContextMenuItem(Category category , bool moveAction)
        {
            Category = category;
            this.moveAction = moveAction;
            Click += MoveCategoryContextMenuItem_Click;
        }

        private void MoveCategoryContextMenuItem_Click(object sender , RoutedEventArgs e)
        {
            if (Category == CategoryManager.GetSelectedCategory())
                return;
            Category selectedCategory = CategoryManager.GetSelectedCategory();
            ShortcutIconGrid shortcutIconGrid = selectedCategory.ShortcutIconGrid;
            foreach (ShortcutIcon shortcutIcon in shortcutIconGrid.SelectedShortcutIconList)
            {
                ShortcutIcon shortcutIconCopy = ShortcutIcon.GetShortcutIconCopy(shortcutIcon);
                shortcutIconCopy.Category = Category;
                Category.ShortcutIconGrid.LoadShortcutIcon(shortcutIconCopy);
                if (moveAction)
                {
                    shortcutIcon.Remove();
                }                    
            }
            if (ConfigManager.AutoSortMode)
            {
                if (moveAction)
                {
                    App.MainAppWindow.SortCategoryShortcutIcons(selectedCategory);
                }
                App.MainAppWindow.SortCategoryShortcutIcons(Category);
            }
        }
    }
}
