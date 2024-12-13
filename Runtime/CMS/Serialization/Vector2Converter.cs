using System;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace JamBootstrap.Serialization
{
    public class Vector2Converter : JsonConverter<Vector2>
    {
        public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                string path = (string)reader.Value;
                return ParseVector2(path);
            }

            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
        {
            writer.WriteValue($"{value.x};{value.y}");
        }
        
        private static Vector2 ParseVector2(string path)
        {
            var parts = path.Split(';');
            if (parts.Length == 2 &&
                float.TryParse(parts[0], out var x) &&
                float.TryParse(parts[1], out var y))
            {
                return new Vector2(x, y);
            }

            throw new JsonSerializationException($"Invalid Vector2 format: {path}");
        }
    }
}