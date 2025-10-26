using System;
using System.Globalization;
using System.Windows.Data;

namespace mywpf
{
    public class StringToBoolConverter : IValueConverter
    {
        public static StringToBoolConverter Instance { get; } = new StringToBoolConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Проверяем, совпадает ли текущее значение (PriceSortDirection) с параметром кнопки ("Asc"/"Desc")
            return value?.ToString() == parameter?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Если кнопка выбрана, возвращаем её параметр
            return (bool)value ? parameter : null;
        }
    }
}