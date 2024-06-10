using System.Globalization;
using System.Windows.Data;

namespace Wpf.Ui.Common.Converters;

public class TimeToLiveToFadingTriggerConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is TimeSpan { TotalSeconds: <= 5, }; //ToDo: Remove this eventually.
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}