using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace ProductivityCake.Converters;

public class DueDateConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTimeOffset date)
        {
            var today = DateTimeOffset.Now.Date;
            var tomorrow = DateTimeOffset.Now.Date.AddDays(1);

            if (date.Date.Equals(today))
                return $"Today, {date:hh:mm tt}";
            else if (date.Date.Equals(tomorrow))
                return $"Tomorrow, {date:hh:mm tt}";
            else
            {
                return $"{date:ddd, hh:mm tt}";
            }

        }
        
        return string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
