using Microsoft.UI.Xaml.Data;
using System;
using System.Globalization;

namespace ConvenienceStore.Converters
{
    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime date)
            {
                return date.ToString("dd/MM/yyyy");
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

}
