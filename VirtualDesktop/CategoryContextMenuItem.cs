using System.Windows.Controls;

namespace VirtualDesktop
{
    public class CategoryContextMenuItem : MenuItem
    {
        private Category category;

        public CategoryContextMenuItem(Category c)
        {
            category = c;
        }

        public Category Category
        {
            get
            {
                return category;
            }
        }
    }
}
