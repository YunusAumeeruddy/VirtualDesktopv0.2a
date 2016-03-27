namespace VirtualDesktop.Actions
{
    public class RemoveShortcutIconAction : Action
    {
        public ShortcutIcon ShortcutIcon;

        public RemoveShortcutIconAction(ShortcutIcon shortcutIcon)
        {
            ShortcutIcon = shortcutIcon;
        }

        public void Redo()
        {
            ShortcutIcon.Remove();
        }

        public void Undo()
        {
            ShortcutIcon.Restore();
        }
    }
}
