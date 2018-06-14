using Newtonsoft.Json.Linq;

namespace TVRename
{
    public static class JSONHelper {
        public static string flatten(JToken ja,string delimiter = ",")
        {
            if (ja == null) return "";

            if (ja.Type == JTokenType.Array)
            {
                JArray ja2 = (JArray)ja;
                string[] values = ja2.ToObject<string[]>();
                return string.Join(delimiter, values);
            }
            else { return ""; }
        }
    }
}
