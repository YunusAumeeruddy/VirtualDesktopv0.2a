namespace VirtualDesktop
{
    public class RowManager
    {
        public static int MinNumberOfRows = 0;
        public static double CellHeight = 82; // 62 (32 x 32 icon) , 82 (48 x 48 icon)
        public static double ShortcutIconExtraVerticalSpace = 0;

        public static void CalculateMinNumberOfRows()
        {
            MainWindow mainWindow = App.MainAppWindow;
            MinNumberOfRows = (int)((mainWindow.gridPanel.Height) / CellHeight);
            MinNumberOfRows -= 1;
        }

        public static void CalculateShortcutIconExtraVerticalSpace()
        {
            MainWindow mainWindow = App.MainAppWindow;
            double heightCoveredByAllCells = (MinNumberOfRows * CellHeight);
            double totalExtraVerticalSpace = mainWindow.gridPanel.Height - heightCoveredByAllCells;
            ShortcutIconExtraVerticalSpace = totalExtraVerticalSpace / (MinNumberOfRows - 1);
        }

        public static double GetShortcutIconCanvasTopMargin(int rowNo)
        {
            return rowNo * GetShortcutIconInclExtraSpaceHeight();
        }

        public static double GetShortcutIconInclExtraSpaceHeight()
        {
            return CellHeight + ShortcutIconExtraVerticalSpace;
        }
    }
}
