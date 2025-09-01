using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace ProductivityCake.Converters;

public class DueDateToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTimeOffset date)
        {
            var today = DateTimeOffset.Now.Date;
            
            if (date.Date.Equals(today))
                return Brushes.Red;
            else
            {
                return Brushes.DodgerBlue;
            }

        }
        return Brushes.Black;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}