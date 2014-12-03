namespace OrderBook.Converters
{
    using DAL.Entities;
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    public class ListItemSelectedBgColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (Status)value;

            switch (status)
            {
                case Status.Neutral:
                    return Brushes.DarkGray;
                case Status.Completed:
                    return Brushes.LawnGreen;
                case Status.Uncompleted:
                    return Brushes.Goldenrod;
                case Status.Busy:
                    return Brushes.Coral;
                default:
                    return Brushes.White;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}