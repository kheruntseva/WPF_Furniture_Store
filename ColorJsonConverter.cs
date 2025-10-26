//using System;
//using System.Windows.Media;
//using Newtonsoft.Json;

//namespace mywpf
//{
//    public class ColorJsonConverter : JsonConverter<Color>
//    {
//        public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
//        {
//            writer.WriteValue(value.ToString());
//        }

//        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue,
//            bool hasExistingValue, JsonSerializer serializer)
//        {
//            try
//            {
//                if (reader.Value == null) return Colors.Black;

//                var colorString = reader.Value.ToString();
//                return (Color)ColorConverter.ConvertFromString(colorString);
//            }
//            catch
//            {
//                return Colors.Black; // Значение по умолчанию
//            }
//        }
//    }
//}