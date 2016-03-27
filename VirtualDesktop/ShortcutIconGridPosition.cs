namespace VirtualDesktop
{
    public class ShortcutIconGridPosition
    {
        public Category Category { get; set; }
        private int rowNo;
        private int colNo;
        public int LeftPos { get; set; }
        public int RightPos { get; set; }
        public int TopPos { get; set; }
        public int BottomPos { get; set; }

        public ShortcutIconGridPosition(Category category , int leftPos , int rightPos , int topPos , int bottomPos)
        {
            Category = category;
            LeftPos = leftPos;
            RightPos = rightPos;
            TopPos = topPos;
            BottomPos = bottomPos;
            CalculateRowNo();
            CalculateColNo();
        }

        public ShortcutIconGridPosition(Category category, int rowNo , int colNo)
        {
            Category = category;
            RowNo = rowNo;
            ColNo = colNo;
        }

        public void CalculateBottomPos()
        {
            BottomPos = Category.ShortcutIconGrid.NoOfRows - rowNo - 1;
        }

        public void CalculateColNo()
        {
            int totalNoCols = ColumnManager.NumberOfColumns;
            if (LeftPos <= RightPos)
                colNo = LeftPos;
            else
            {
                colNo = (totalNoCols - 1) - RightPos;
            }

            if (colNo < 0)
                colNo = 0;
            else if (ColNo >= totalNoCols)
                colNo = totalNoCols - 1;
        }

        public void CalculateLeftPos()
        {
            LeftPos = colNo;
        }

        public void CalculateRightPos()
        {
            RightPos = ColumnManager.NumberOfColumns - colNo - 1;
        }

        public void CalculateRowNo()
        {
            int totalNoRows = Category.ShortcutIconGrid.NoOfRows;

            if (TopPos <= BottomPos)
                rowNo = TopPos;
            else
            {
                rowNo = (totalNoRows - 1) - BottomPos;
            }

            if (rowNo < 0)
                rowNo = 0;
        }

        public void CalculateTopPos()
        {
            TopPos = rowNo;
        }

        public int ColNo
        {
            get
            {
                return colNo;
            }
            set
            {
                colNo = value;
                CalculateLeftPos();
                CalculateRightPos();
            }
        }

        public int RowNo
        {
            get
            {
                return rowNo;
            }
            set
            {
                rowNo = value;
                CalculateTopPos();
                CalculateBottomPos();
            }
        }

        public override string ToString()
        {
            return "Row no : " + RowNo + " , Column no : " + ColNo;
        }
    }
}
