namespace VirtualDesktop
{
    public class MathUtils
    {
        public static double GetDoubleValue(double value)
        {
            if (double.IsNaN(value))
                return 0;
            return value;
        }

        public static double GetMaxOf3Double(double num1 , double num2 , double num3)
        {
            double max = num1;
            if (num2 > max)
            {
                max = num2;
            }
            if(num3 > max)
            {
                max = num3;
            }
            return max;
        }
    }
}
