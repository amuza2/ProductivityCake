using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace ProductivityCake.Converters;

public class BoolToStartPauseConverter : IValueConverter
{
    public string PlayText { get; set; } = "Start";
    public string PauseText { get; set; } = "Pause";
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isRunning)
        {
            return isRunning ? PauseText : PlayText;
        }
        return PlayText;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
