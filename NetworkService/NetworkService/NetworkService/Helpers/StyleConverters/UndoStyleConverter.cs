using NetworkService.ViewModel;
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
    public class UndoStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is ActionType actionType && actionType == ActionType.NoAction)
            {
                return Application.Current.FindResource("UndoButtonUnavailableStyle");
            }
            else
            {
                return Application.Current.FindResource("UndoButtonStyle");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
