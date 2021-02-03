namespace NepseClient.Modules.Stocks.Utils
{
    public static class NumberUtils
    {
        public static string GetPointChangedText(double number)
        {
            if (number < 0)
            {
                return string.Format("{0:N}", number);
            }
            else
            {
                return string.Format("+{0:N}", number);
            }
        }

        public static string GetPercentChangedText(double number)
        {
            if (number < 0)
            {
                return string.Format("{0:P}", number);
            }
            else
            {
                return string.Format("+{0:P}", number);
            }
        }

        public static string GetMarketCapText(double number)
        {
            var newNumber = number;
            var modifier = "";
            if (newNumber > 1000)
            {
                newNumber /= 1000;
                modifier = "K";
            }
            if (newNumber > 1000)
            {
                newNumber /= 1000;
                modifier = "M";
            }
            if (newNumber > 1000)
            {
                newNumber /= 1000;
                modifier = "B";
            }
            if (newNumber > 1000)
            {
                newNumber /= 1000;
                modifier = "T";
            }

            return string.Format("{0:F1}{1}", newNumber, modifier);
        }
    }
}
