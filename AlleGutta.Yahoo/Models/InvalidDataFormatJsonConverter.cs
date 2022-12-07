using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AlleGutta.Yahoo.Models;

public class InvalidDataFormatJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType) => objectType == typeof(DateTime?);

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (objectType == typeof(DateTime?) && reader.TokenType == JsonToken.Integer)
        {
            var dateString = new JValue(reader.Value);
            if (long.TryParse(dateString.ToString(), out var numberDate))
            {
                numberDate *= 1000;
                return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    .AddMilliseconds(numberDate)
                    .ToLocalTime();
            }
        }
        return null;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}