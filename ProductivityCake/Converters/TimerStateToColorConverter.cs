using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using ProductivityCake.ViewModels;

namespace ProductivityCake.Converters;

public class TimerStateToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is TimerState state)
        {
            var colorString = state switch
            {
                TimerState.Work => "#16A34A",           // Green
                TimerState.LongWork => "#059669",      // Darker Green
                TimerState.ShortBreak => "#3B82F6",    // Blue
                TimerState.LongBreak => "#9333EA",     // Purple
                _ => "#374151"                         // Default Gray
            };
            
            return Color.Parse(colorString);
        }
        
        return Color.Parse("#374151");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
