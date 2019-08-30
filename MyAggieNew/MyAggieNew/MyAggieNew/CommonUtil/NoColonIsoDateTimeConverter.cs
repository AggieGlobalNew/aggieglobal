using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MyAggieNew
{
    public class NoColonIsoDateTimeConverter : IsoDateTimeConverter
    {
        public NoColonIsoDateTimeConverter()
        {
            DateTimeFormat = "yyyy'-'MM'-'dd'T00:00:000Z'";
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DateTime)
            {
                var dateTime = (DateTime)value;
                var text = dateTime.ToString(DateTimeFormat);
                text = text.Remove(text.Length - 3, 1);
                writer.WriteValue(text);
            }
            else
            {
                throw new JsonSerializationException("Unexpected value when converting date. Expected DateTime");
            }
        }
    }
}