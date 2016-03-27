namespace VirtualDesktop.Actions
{
    public class RemoveFreeShortcutIconPositionAction : Action
    {
        public ShortcutIconGrid ShortcutIconGrid;
        public ShortcutIconGridPosition GridPosition;

        public RemoveFreeShortcutIconPositionAction(ShortcutIconGrid shortcutIconGrid , ShortcutIconGridPosition pos)
        {
            ShortcutIconGrid = shortcutIconGrid;
            GridPosition = pos;
        }

        public void Redo()
        {
            ShortcutIconGrid.RemoveShortcutIconFreePosition(GridPosition);
        }

        public void Undo()
        {
            ShortcutIconGrid.AddShortcutIconFreePosition(GridPosition);
        }
    }
}
