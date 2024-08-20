using System.Globalization;


namespace KidsChoresApp.Converters
{
    public class CurrencySymbolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string currency)
            {
                var cultureInfo = currency switch
                {
                    "USD" => new CultureInfo("en-US"),
                    "EUR" => new CultureInfo("en-IE"),
                    "GBP" => new CultureInfo("en-GB"),
                    _ => new CultureInfo("en-US")
                };

                return cultureInfo.NumberFormat.CurrencySymbol;
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
