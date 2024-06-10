using System.Globalization;
using System.Windows.Data;

namespace Wpf.Ui.Common.Converters;

public class TimeSpanToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is TimeSpan timeSpan) { return timeSpan.ToString(); }
        return null;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string timeString && TimeSpan.TryParse(timeString, out var timeSpan)) { return timeSpan; }
        return TimeSpan.Zero;
    }
}