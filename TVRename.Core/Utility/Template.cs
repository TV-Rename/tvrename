using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Scriban;
using Scriban.Runtime;

namespace TVRename.Core.Utility
{
    public static class TemplateExtentions
    {
        public static string Template(this string tpl, object obj)
        {
            ScriptObject script = new ScriptObject();
            script.Import(typeof(TemplateHelpers));
            script.Import(obj);

            return tpl.Template(script);
        }

        public static string Template(this string tpl, Dictionary<string, object> objs)
        {
            ScriptObject script = new ScriptObject();
            script.Import(typeof(TemplateHelpers));

            foreach (KeyValuePair<string, object> obj in objs)
            {
                script.Add(obj.Key, obj.Value);
            }
            
            return tpl.Template(script);
        }

        public static string Template(this string tpl, ScriptObject script)
        {
            return Scriban.Template.Parse(tpl).Render(new TemplateContext(script));
        }
    }

    // {{show.name}} - S{{season.number | pad}}{{episode.numbers | pad_each | prefix_each 'E' | join '-'}} - {{episode.names | trim_each | merge_episode_names | join ' + '}}

    [PublicAPI]
    public static class TemplateHelpers
    {
        public static string Pad(string value)
        {
            return value.PadLeft(2, '0');
        }

        public static IEnumerable<string> PadEach(IEnumerable values)
        {
            foreach (object value in values)
            {
                yield return Pad(value?.ToString());
            }
        }

        public static string Prefix(string value, string prefix)
        {
            return prefix + value;
        }

        public static IEnumerable<string> PrefixEach(IEnumerable values, string prefix)
        {
            foreach (object value in values)
            {
                yield return Prefix(value?.ToString(), prefix);
            }
        }

        public static string Join(IEnumerable values, string seperator)
        {
            return string.Join(seperator, values);
        }

        public static string Trim(string value)
        {
            return value.Trim();
        }

        public static IEnumerable<string> TrimEach(IEnumerable values)
        {
            foreach (object value in values)
            {
                yield return Trim(value?.ToString());
            }
        }
    }
}
