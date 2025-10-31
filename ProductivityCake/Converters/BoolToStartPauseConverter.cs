using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace ProductivityCake.Converters;

public class BoolToStartPauseConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isRunning)
        {
            return isRunning ? "Pause" : "Start";
        }
        return "Start";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
