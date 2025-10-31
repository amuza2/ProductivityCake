using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace ProductivityCake.Converters;

public class ProgressToAngleConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double progress)
        {
            // Convert 0-1 progress to 0-360 degrees
            return progress * 360.0;
        }
        return 0.0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
