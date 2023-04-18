using System;
using System.Globalization;
using System.Windows.Data;

namespace WPFClient.GameCatalog.Converter
{
    public class GameToggleButtonTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                if (boolValue)
                {
                    return "Remove";
                }
                else
                {
                    return "Add";
                }
            }

            throw new Exception($"Can't convert value:{value} to type: {targetType.Name}");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
