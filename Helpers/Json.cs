using Newtonsoft.Json;

namespace SiphoEngine.Helpers
{
    public static class Json
    {
        public static string ToJson(object? value, JsonSerializerSettings settings = default, Formatting formatting = Formatting.None)
        {
            if (settings is null)
            {
                settings = new JsonSerializerSettings();
            }
            return JsonConvert.SerializeObject(value, formatting, settings: settings);
        }

        public static T FromJson<T>(string value, JsonSerializerSettings? settings)
        {
            return JsonConvert.DeserializeObject<T>(value, settings);
        }
    }
}
