using System;
using Unity.Plastic.Newtonsoft.Json;

namespace JamBootstrap.Serialization
{
    public class CMSEntityConverter : JsonConverter<CMSEntity>
    {
        public override CMSEntity ReadJson(JsonReader reader, Type objectType, CMSEntity existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                string path = (string)reader.Value;
                return CMS.Get<CMSEntity>(path);
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, CMSEntity value, JsonSerializer serializer)
        {
            writer.WriteValue(value.id);
        }
    }
}