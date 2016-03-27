namespace VirtualDesktop.Actions
{
    public class RenameShortcutIconAction : Action
    {
        public ShortcutIcon ShortcutIcon;
        private string oldName;
        private string newName;
        private bool fileRenamed = false;

        public RenameShortcutIconAction(ShortcutIcon shortcutIcon , string oldName , string newName , bool fileRenamed)
        {
            ShortcutIcon = shortcutIcon;
            this.oldName = oldName;
            this.newName = newName;
            this.fileRenamed = fileRenamed;
        }

        public void Redo()
        {
            ShortcutIcon.RenameLabelText(newName);
            if (fileRenamed)
            {
                ShortcutIcon.RenameFile(newName);
            }
        }

        public void Undo()
        {
            if (fileRenamed)
            {
                ShortcutIcon.RenameFile(oldName);
            }
            ShortcutIcon.RenameLabelText(oldName);
        }
    }
}
