using Newtonsoft.Json;

namespace JamBootstrap.Serialization
{
    public class JsonSerializerSettingsBuilder
    {
        public static JsonSerializerSettings Builder()
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            settings.Converters.Add(new Vector2Converter());
            settings.Converters.Add(new CMSEntityConverter());
            settings.Converters.Add(new ColorConverter());
            return settings;
        }
    }
}