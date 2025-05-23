using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CavalryCivil3DPlugin.WPFSupportFunctions.Supports
{
    public class EmptyToZeroConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() ?? "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = value as string;
            if (string.IsNullOrWhiteSpace(str))
                return 0.0;

            // Allow intermediate input like ".", "-", etc.
            if (str == "." || str.EndsWith("."))
                return Binding.DoNothing;

            if (double.TryParse(str, out double result))
            {
                return result;
            }

            else
            {
                return 0.0;
            }
            // Invalid number (e.g. letters), ignore input
            //return Binding.DoNothing;
        }
    }
}
