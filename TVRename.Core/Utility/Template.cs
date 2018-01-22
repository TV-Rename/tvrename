using System.Collections.Generic;
using JetBrains.Annotations;
using Scriban;
using Scriban.Runtime;

namespace TVRename.Core.Utility
{
    public static class TemplateExtentions
    {
        public static string Template(this string str, object obj)
        {
            ScriptObject tplScript = new ScriptObject();
            tplScript.Import(typeof(TemplateHelpers));
            tplScript.Import(obj);

            return str.Template(tplScript);
        }

        public static string Template(this string str, Dictionary<string, object> objs)
        {
            ScriptObject tplScript = new ScriptObject();
            tplScript.Import(typeof(TemplateHelpers));

            foreach (KeyValuePair<string, object> obj in objs)
            {
                tplScript.Add(obj.Key, obj.Value);
            }
            
            return str.Template(tplScript);
        }

        public static string Template(this string str, ScriptObject script)
        {
            return Scriban.Template.Parse(str).Render(new TemplateContext(script));
        }
    }

    [PublicAPI]
    public static class TemplateHelpers
    {
        public static string Pad(object value)
        {
            return value?.ToString().PadLeft(2, '0');
        }
    }
}
