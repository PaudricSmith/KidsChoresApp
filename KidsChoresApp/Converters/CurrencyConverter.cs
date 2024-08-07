using System.Globalization;


namespace KidsChoresApp.Converters
{
    public class CurrencyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2)
                throw new ArgumentException("Expected two binding values: money and currency.");

            if (values[0] is decimal money && values[1] is string currency)
            {
                var cultureInfo = currency switch
                {
                    "USD" => new CultureInfo("en-US"),
                    "EUR" => new CultureInfo("en-IE"),
                    "GBP" => new CultureInfo("en-GB"),
                    _ => new CultureInfo("en-US")
                };

                return string.Format(cultureInfo, "{0:C}", money);
            }

            return string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}