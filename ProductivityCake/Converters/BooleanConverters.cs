using Avalonia.Data.Converters;
using Avalonia.Media;

namespace ProductivityCake.Converters
{
    public static class BooleanConverters
    {
        public static readonly IValueConverter TrueToItalic =
            new FuncValueConverter<bool, FontStyle>(b => b ? FontStyle.Italic : FontStyle.Normal);
        
        // Change IBrush to SolidColorBrush
        public static readonly IValueConverter TrueToLightGray =
            new FuncValueConverter<bool, SolidColorBrush>(b => 
                b ? new SolidColorBrush(Colors.Gray) : new SolidColorBrush(Colors.Black));
        
        // Converter for expander icon (▼ when expanded, ▶ when collapsed)
        public static readonly IValueConverter ToExpanderIcon =
            new FuncValueConverter<bool, string>(b => b ? "▼" : "▶");
    }
}