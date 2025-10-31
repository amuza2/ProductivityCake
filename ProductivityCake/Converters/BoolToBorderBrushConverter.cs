using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace ProductivityCake.Converters;

public class BoolToBorderBrushConverter : IValueConverter
{
    public string? ActiveColor { get; set; }
    public string InactiveColor { get; set; } = "#4B5563";

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isActive && isActive && !string.IsNullOrEmpty(ActiveColor))
        {
            return new SolidColorBrush(Color.Parse(ActiveColor));
        }
        return new SolidColorBrush(Color.Parse(InactiveColor));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
