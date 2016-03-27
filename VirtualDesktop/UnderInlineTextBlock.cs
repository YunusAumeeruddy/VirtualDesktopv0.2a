using System.Windows.Controls;
using System.Windows.Documents;

namespace VirtualDesktop
{
    public class UnderInlineTextBlock : TextBlock
    {
        public void UnderlineText(string searchText)
        {
            if (searchText == null)
                return;

            searchText = searchText.ToLower();

            if (Text == null || Text.Length == 0 || !Text.ToLower().Contains(searchText))
                return;

            string wholeText = Text;
            Text = "";
            int currentIndex = 0;
            int searchTextLength = searchText.Length;
            while (currentIndex < wholeText.Length)
            {
                if (wholeText.Substring(currentIndex).Length < searchTextLength)
                {
                    Inlines.Add(new Run(wholeText.Substring(currentIndex)));
                    break;
                }
                else
                {
                    string substring = wholeText.Substring(currentIndex , searchTextLength);
                    if (substring.ToLower().Equals(searchText))
                    {
                        Inlines.Add(new Underline(new Run(substring)));
                        currentIndex += searchTextLength;
                    }
                    else
                    {
                        Inlines.Add(new Run(wholeText.Substring(currentIndex , 1)));
                        currentIndex++;
                    }
                }
            }
        }
    }
}
