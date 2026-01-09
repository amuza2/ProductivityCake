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
                TimerState.Work => "#22d3ee",            // Cyan (primary accent)
                TimerState.LongWork => "#a78bfa",        // Purple (secondary)
                TimerState.ShortBreak => "#4ade80",      // Green (success)
                TimerState.LongBreak => "#fbbf24",       // Amber/Yellow (warm)
                _ => "#22d3ee"                           // Default Cyan
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
