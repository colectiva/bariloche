using La_Colectiva.Models.Classic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace La_Colectiva.Views.Converters
{
    public class OperationStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch(value) {
                case OperationStatus.CONFIRMED:
                    return "Confirmado";
                case OperationStatus.DELAYED:
                    return "Con demoras";
                case OperationStatus.CANCELLED:
                    return "Cancelado";
                default:
                    return "";
            }
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
}
