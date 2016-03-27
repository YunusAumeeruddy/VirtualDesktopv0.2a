namespace VirtualDesktop.Actions
{
    public class SwapCategoryAction : Action
    {
        public Category Category1;
        public Category Category2;

        public SwapCategoryAction(Category category1 , Category category2)
        {
            Category1 = category1;
            Category2 = category2;
        }

        public void Redo()
        {
            CategoryManager.SwapCategory(Category1 , Category2);
        }

        public void Undo()
        {
            CategoryManager.SwapCategory(Category1 , Category2);
        }
    }
}
