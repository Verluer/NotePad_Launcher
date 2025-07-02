using System.Globalization;
using System.Windows.Data;

namespace NotePad_Launcher.MVVM.Commands;

public class OptionToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string selectedOption && parameter is string option)
        {
            return selectedOption == option;
        }
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? parameter : null;
    }
}