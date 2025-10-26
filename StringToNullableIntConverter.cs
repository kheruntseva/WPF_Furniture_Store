
//using System;
//using System.Globalization;
//using System.Windows.Data;

//namespace mywpf
//{
//    public class StringToNullableIntConverter : IValueConverter
//    {
//        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            if (value is int ? nullableInt && nullableInt.HasValue)
//            {
//                return nullableInt.Value.ToString();
//            }
//            return null;
//        }

//        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            if (value is string str && int.TryParse(str, out int result))
//            {
//                return result;
//            }
//            return null;
//        }
//    }
//}