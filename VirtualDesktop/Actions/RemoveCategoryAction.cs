namespace VirtualDesktop.Actions
{
    public class RemoveCategoryAction : Action
    {
        public Category Category;

        public RemoveCategoryAction(Category category)
        {
            Category = category;
        }

        public void Redo()
        {
            CategoryManager.RemoveCategory(Category);
        }

        public void Undo()
        {
            CategoryManager.RestoreCategory(Category);
        }
    }
}
