using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.UI;

namespace ConvenienceStore.Converters
{
    public class AttendanceStatusToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string status && parameter is string targetStatus)
            {
                if (status == targetStatus)
                {
                    return new SolidColorBrush(Color.FromArgb(255, 0, 128, 0)); // Green
                }
            }
            return new SolidColorBrush(Colors.Transparent); // Transparent
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}