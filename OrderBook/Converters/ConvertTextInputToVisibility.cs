using System;
using System.Windows;
using System.Windows.Data;

namespace OrderBook.Converters
{
    public class TextInputToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            // Always test MultiValueConverter inputs for non-null
            // (to avoid crash bugs for views in the designer)
            if (!(values[0] is bool) || !(values[1] is bool)) return Visibility.Visible;
            var hasText = !(bool) values[0];
            var hasFocus = (bool) values[1];

            if (hasFocus || hasText)
                return Visibility.Collapsed;

            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}