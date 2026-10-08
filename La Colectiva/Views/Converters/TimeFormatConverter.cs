using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace La_Colectiva.Views.Converters
{
    public class TimeFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return string.Empty;
            if (value is TimeSpan ts) return ts.ToString(@"hh\:mm");
            return "00:00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string lang)
        {
            throw new NotImplementedException();
        }
    }
}
