using System;
using Engine.Math;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace JamBootstrap.Serialization
{
    public class ColorConverter : JsonConverter<Color>
    {
        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                string colorString = (string)reader.Value;
                return ParseColor(colorString);
            }

            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
        {
            writer.WriteValue(ColorUtility.ToHtmlStringRGBA(value));
        }

        private static Color ParseColor(string colorString)
        {
            colorString = $"#{colorString}";
            return colorString.ParseColor();
        }
    }
}