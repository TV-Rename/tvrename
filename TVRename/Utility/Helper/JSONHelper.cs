using Newtonsoft.Json.Linq;

namespace TVRename
{
    public static class JsonHelper {
        public static string Flatten(JToken ja,string delimiter = ",")
        {
            if (ja == null) return "";

            if (ja.Type != JTokenType.Array) return "";

            JArray ja2 = (JArray)ja;
            string[] values = ja2.ToObject<string[]>();
            return string.Join(delimiter, values);
        }
    }
}
