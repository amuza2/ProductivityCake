using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;
using ProductivityCake.Models;

namespace ProductivityCake.Converters;

public class ContainsItemConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count == 2 && values[0] is Category tag && values[1] is IEnumerable<Category> selectedTags)
        {
            return selectedTags.Any(t => t.Id == tag.Id);
        }
        return false;
    }
}
