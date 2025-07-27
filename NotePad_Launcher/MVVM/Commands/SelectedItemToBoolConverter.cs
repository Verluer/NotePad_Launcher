using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NotePad_Launcher.MVVM.Commands
{
    public class SelectedItemToBoolConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2) return false;

            var selectedItem = values[0];
            var currentItem = values[1];

            return Equals(selectedItem, currentItem);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            // При обратном связывании, если IsChecked == true, вернуть текущий элемент как выбранный
            if (value is bool isChecked && isChecked)
            {
                return new object[] { Binding.DoNothing, Binding.DoNothing };
            }
            return new object[] { Binding.DoNothing, Binding.DoNothing };
        }
    }
}
