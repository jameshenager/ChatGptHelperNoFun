using System.Globalization;
using System.Windows.Data;

namespace Wpf.Ui.Common.Converters;

public class BoolToMaxHeightConverter : IValueConverter //ToDo: Should be rewritten as IMultiValueConverter
{
    // Convert method now checks for the isExpanded boolean and uses a parameter for the collapsed height.
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // Check if the value is a boolean and represents the expanded state.
        if (value is not bool isExpanded) { return 50.0; }
        // If expanded, return PositiveInfinity to allow for any height.
        if (isExpanded) { return double.PositiveInfinity; }

        if (parameter is string paramString && double.TryParse(paramString, out var collapsedHeight)) { return collapsedHeight; }

        return 50.0;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}

public class BoolToMaxHeightMultiConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length != 2) { return 50.0; }
        if (values[0] is not bool isExpanded) { return 50.0; }
        if (isExpanded) { return double.PositiveInfinity; }
        if (values[1] is not double collapsedHeight) { return 50.0; }
        return collapsedHeight;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
}