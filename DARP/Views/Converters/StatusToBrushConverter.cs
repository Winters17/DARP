using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace DARP.Views.Converters
{

    using EnumStatusType = Constants.StatusType;
    public class StatusToBrushConverter : IValueConverter
    {

            Brush NoneValue = null;
            Brush OkValue = Brushes.LightGreen;
            Brush WarningValue = Brushes.Orange;
            Brush ErrorValue = Brushes.IndianRed;


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((EnumStatusType)value == EnumStatusType.Ok) return OkValue;
            else if ((EnumStatusType)value == EnumStatusType.Warning) return WarningValue;
            else if ((EnumStatusType)value == EnumStatusType.Error) return ErrorValue;
            else if ((EnumStatusType)value == EnumStatusType.None) return NoneValue;
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
