using Microsoft.UI.Xaml.Data;
using System;
using System.Diagnostics;

namespace ConvenienceStore.Converters
{
    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value is DateTime date)
                {
                    return date.ToString("dd/MM/yyyy");
                }
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DateConverter Error: {ex.Message}, Value type is {value?.GetType()}");
                return null;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string dateString && DateTime.TryParse(dateString, out DateTime date))
            {
                return date;
            }
            return null;
        }
    }
}