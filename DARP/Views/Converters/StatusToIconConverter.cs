using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DARP.Views.Converters
{
    using System.Globalization;
    using System.Windows.Data;
    using EnumStatusType = Constants.StatusType;

    public class StatusToIconConverter : IValueConverter
    {
        string NoneValue = null;
        string OkValue = @"/DARP;component/Resources/ok_16.png";
        string WarningValue = @"/DARP;component/Resources/warning_16.png";
        string ErrorValue = @"/DARP;component/Resources/error_16.png";

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
