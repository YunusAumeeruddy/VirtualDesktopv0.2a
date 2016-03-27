namespace VirtualDesktop.Actions
{
    public class AddFreeShortcutIconPositionAction : Action
    {
        public ShortcutIconGrid ShortcutIconGrid;
        public ShortcutIconGridPosition GridPosition;

        public AddFreeShortcutIconPositionAction(ShortcutIconGrid shortcutIconGrid , ShortcutIconGridPosition pos)
        {
            ShortcutIconGrid = shortcutIconGrid;
            GridPosition = pos;
        }

        public void Redo()
        {
            ShortcutIconGrid.AddShortcutIconFreePosition(GridPosition);
        }

        public void Undo()
        {
            ShortcutIconGrid.RemoveShortcutIconFreePosition(GridPosition);
        }
    }
}
