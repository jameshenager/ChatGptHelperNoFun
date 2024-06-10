using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Wpf.Ui.Common.Converters;

//ToDo: https://github.com/michael-damatov/lambda-converters/?tab=readme-ov-file#lambda-value-converters 

public class BooleanToBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var defaultColor = Colors.White;
        var trueColor = parameter?.ToString() ?? nameof(Colors.Red);

        if (value is not bool boolValue) { return new SolidColorBrush(defaultColor); }
        var color = boolValue ? (Color)ColorConverter.ConvertFromString(trueColor) : defaultColor;
        return new SolidColorBrush(color);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}