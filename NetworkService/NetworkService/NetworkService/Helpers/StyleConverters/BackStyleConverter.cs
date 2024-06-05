using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace NetworkService.Helpers.StyleConverters
{
    public class BackStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if((value as BindableBase) == null)
            {
                return Application.Current.FindResource("BackButtonUnavailableStyle");
            }
            else
            {
                return Application.Current.FindResource("BackButtonStyle");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
