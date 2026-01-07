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
                TimerState.Work => "#4f772d",           // Fern
                TimerState.LongWork => "#31572c",      // Hunter Green
                TimerState.ShortBreak => "#90a955",    // Palm Leaf
                TimerState.LongBreak => "#ecf39e",     // Lime Cream
                _ => "#31572c"                         // Default Hunter Green
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
