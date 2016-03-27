namespace VirtualDesktop.Actions
{
    public class AddCategoryAction : Action
    {
        public Category Category;

        public AddCategoryAction(Category category)
        {
            Category = category;
        }

        public void Redo()
        {
            CategoryManager.RestoreCategory(Category);
        }

        public void Undo()
        {
            CategoryManager.RemoveCategory(Category);
        }
    }
}
