namespace VirtualDesktop.Actions
{
    public class AddShortcutIconAction : Action
    {
        public ShortcutIcon ShortcutIcon;

        public AddShortcutIconAction(ShortcutIcon shortcutIcon)
        {
            ShortcutIcon = shortcutIcon;
        }

        public void Redo()
        {
            ShortcutIcon.Restore();
        }

        public void Undo()
        {
            ShortcutIcon.Remove();
        }
    }
}
