﻿namespace OrderBook.Converters
{
    using DAL.Entities;
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    public class ListItemBgColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (Status) value;

            switch (status)
            {
                case Status.Neutral:
                    return Brushes.Gainsboro;
                case Status.Completed:
                    return Brushes.GreenYellow;
                case Status.Uncompleted:
                    return Brushes.Orange;
                case Status.Busy:
                    return Brushes.Tomato;
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