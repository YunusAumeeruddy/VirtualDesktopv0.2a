namespace VirtualDesktop
{
    public class ColumnManager
    {
        public static double ShortcutIconExtraHorizontalSpace = 0;
        public static int MinNoOfCols = 0;
        private static int noOfCols = 0;

        public static double CellWidth = 102; // 86 (32 x 32 icon) , 102 (48 x 48 icon)

        public static void CalculateMinNumberOfColumns()
        {
            MainWindow mainWindow = App.MainAppWindow;
            MinNoOfCols = (int)((mainWindow.scroll_ShortcutIconGridCanvas.Width - 20)
                                 / CellWidth);
            MinNoOfCols -= 1;
            if (noOfCols == 0)
                noOfCols = MinNoOfCols;
        }

        public static void CalculateShortcutIconExtraHorizontalSpace()
        {
            MainWindow mainWindow = App.MainAppWindow;
            double widthCoveredByAllCells = (noOfCols * CellWidth);
            double totalExtraHorizontalSpace = mainWindow.scroll_ShortcutIconGridCanvas.Width
                                                - 20
                                                - widthCoveredByAllCells;
            ShortcutIconExtraHorizontalSpace = totalExtraHorizontalSpace / (noOfCols - 1);
        }

        public static double GetShortcutIconCanvasLeftMargin(int colNo)
        {
            double canvasLeftMargin = (colNo * GetShortcutIconInclExtraSpaceWidth())
                                 + App.MainAppWindow.scroll_ShortcutIconGridCanvas.Margin.Left;

            return canvasLeftMargin;
        }

        public static double GetShortcutIconInclExtraSpaceWidth()
        {
            return CellWidth + ShortcutIconExtraHorizontalSpace;
        }

        public static int NumberOfColumns
        {
            get
            {
                return noOfCols;
            }
            set
            {
                noOfCols = value;
                if (noOfCols < MinNoOfCols)
                    noOfCols = MinNoOfCols;
            }
        }
    }
}
