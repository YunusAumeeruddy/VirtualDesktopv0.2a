namespace VirtualDesktop
{
    public class StringUtils
    {
        private StringUtils() { }

        public static bool EqualsIgnoreCase(string str1 , string str2)
        {
            return str1.ToLower().Equals(str2.ToLower());
        }
    }
}
