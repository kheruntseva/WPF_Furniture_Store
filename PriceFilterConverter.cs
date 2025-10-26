using System;
using System.Globalization;
using System.Windows.Data;

namespace mywpf
{
    public class PriceFilterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            string stringValue = value.ToString();

            if (stringValue == "None")
                return LanguageManager.GetTranslation("UI.WithoutSort");
            else if (stringValue == "Asc")
                return LanguageManager.GetTranslation("UI.SortAsc");
            else if (stringValue == "Desc")
                return LanguageManager.GetTranslation("UI.SortDesc");

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            string translatedValue = value.ToString();

            if (translatedValue == LanguageManager.GetTranslation("UI.WithoutSort"))
                return "None";
            if (translatedValue == LanguageManager.GetTranslation("UI.SortAsc"))
                return "Asc";
            if (translatedValue == LanguageManager.GetTranslation("UI.SortDesc"))
                return "Desc";

            return value;
        }
    }
}