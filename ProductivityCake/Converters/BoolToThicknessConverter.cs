using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace ProductivityCake.Converters;

public class BoolToThicknessConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isActive && isActive)
        {
            return new Thickness(3); // Active: thicker border
        }
        return new Thickness(2); // Inactive: normal border
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
