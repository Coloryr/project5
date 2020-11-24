using Lib;
using System;
using System.Windows.Data;

namespace IoTMcuEdit
{
    public class FontColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            FontSelfColor s = (FontSelfColor)value;
            return s == (FontSelfColor)int.Parse(parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isChecked = (bool)value;
            if (!isChecked)
            {
                return null;
            }
            return (FontSelfColor)int.Parse(parameter.ToString());
        }
    }
}
