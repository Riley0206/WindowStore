using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.UI;

namespace ConvenienceStore.Converters
{
    public class AttendanceStatusToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string targetStatus = parameter as string;
            if (value is string status && parameter is string)
            {
                if (status == targetStatus)
                {
                    return $"[ {targetStatus} ]";
                }
            }
            return targetStatus;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}