using La_Colectiva.Models.Classic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace La_Colectiva.Views.Converters
{
    public class RouteStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch (value) {
                case RouteStatus.OK:
                    return "Programado";
                case RouteStatus.UNKNOWN:
                    return "Programado";
                case RouteStatus.LOW_TIDE:
                    return "Sujeto a marea baja";
                case RouteStatus.HIGH_TIDE:
                    return "Sujeto a marea alta";
                default:
                    return "";
            };
    }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
