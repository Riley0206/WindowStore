using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using System;

namespace ConvenienceStore.Converters
{
    public class NullToGreyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                // Trả về màu xám khi giá trị null
                return new SolidColorBrush(Microsoft.UI.Colors.Gray);
            }
            // Trả về màu mặc định (đen) khi có giá trị
            return new SolidColorBrush(Microsoft.UI.Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
