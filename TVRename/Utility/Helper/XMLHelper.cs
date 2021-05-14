using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using JetBrains.Annotations;
using NLog;

namespace TVRename
{
    public static class XmlHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void WriteStringsToXml([NotNull] IEnumerable<string> strings, [NotNull] XmlWriter writer, [NotNull] string elementName, [NotNull] string stringName)
        {
            writer.WriteStartElement(elementName);
            writer.WriteStringsToXml(stringName,strings);
            writer.WriteEndElement();
        }

        public static void WriteStringsToXml(this XmlWriter writer,  [NotNull] string elementName, [NotNull] IEnumerable<string> strings)
        {
            foreach (string ss in strings)
            {
                writer.WriteStartElement(elementName);
                writer.WriteValue(ss);
                writer.WriteEndElement();
            }
        }

        [NotNull]
        public static string ReadStringFixQuotesAndSpaces([NotNull] string s)
        {
            string res = s.Replace("\\'", "'");
            res = res.Replace("\\\"", "\"");
            res = res.Trim();
            return res;
        }

        public static void WriteElement(this XmlWriter writer, string elementName, string? value)
        {
            WriteElement(writer, elementName, value, false);
        }

        public static void WriteElement(this XmlWriter writer, string elementName, string? value,bool ignoreIfBlank)
        {
            if (ignoreIfBlank && string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            writer.WriteStartElement(elementName);
            try
            {
                writer.WriteValue(value ?? string.Empty);
            }
            catch (ArgumentException e)
            {
                Logger.Error($"Cound not write {elementName} with value {value} because {e.Message}");
            }

            writer.WriteEndElement();
        }

        public static void WriteElement([NotNull] this XmlWriter writer, [NotNull] string elementName, double value)
        {
            WriteElement(writer, elementName, value, null);
        }

        public static void WriteElement([NotNull] this XmlWriter writer, [NotNull] string elementName, double? value,string? format)
        {
            if (!value.HasValue)
            {
                return;
            }
            writer.WriteStartElement(elementName);
            if (format is null)
            {
                writer.WriteValue(value);
            }
            else
            {
                writer.WriteValue(value.Value.ToString(format));
            }
            writer.WriteEndElement();
        }

        public static void WriteElement(this XmlWriter writer, string elementName, int value)
        {
            WriteElement(writer, elementName, value, false);
        }

        public static void WriteElement(this XmlWriter writer, string elementName, int value,bool ignoreZero)
        {
            if (ignoreZero && value == 0)
            {
                return;
            }

            writer.WriteStartElement(elementName);
            writer.WriteValue(value);
            writer.WriteEndElement();
        }
        public static void WriteElement([NotNull] this XmlWriter writer, [NotNull] string elementName, bool value)
        {
            writer.WriteStartElement(elementName);
            writer.WriteValue(value);
            writer.WriteEndElement();
        }
        public static void WriteElement([NotNull] this XmlWriter writer, [NotNull] string elementName, DateTime? value)
        {
            writer.WriteStartElement(elementName);
            if (value != null)
            {
                writer.WriteValue(value);
            }

            writer.WriteEndElement();
        }

        public static void WriteAttributeToXml([NotNull] this XmlWriter writer, [NotNull] string attributeName, [NotNull] string value)
        {
            writer.WriteStartAttribute(attributeName);
            writer.WriteValue(value);
            writer.WriteEndAttribute();
        }

        public static XElement GetOrCreateElement([NotNull] this XElement root, string elementName)
        {
            if (root.Elements(elementName).Any())
            {
                return root.Elements(elementName).First();
            }
            XElement e = new XElement(elementName);
            root.Add(e);
            return e;
        }

        public static XElement GetOrCreateElement([NotNull] this XElement root, string elementName, string name, string value)
        {
            if (root.Elements(elementName).Any(el => el.HasAttribute(name,value)))
            {
                return root.Elements(elementName).First(el => el.HasAttribute(name,value));
            }
            XElement e = new XElement(elementName);
            root.Add(e);
            return e;
        }

        public static void WriteAttributeToXml([NotNull] this XmlWriter writer, [NotNull] string attributeName, DateTime?  value)
        {
            writer.WriteStartAttribute(attributeName);
            if (value != null)
            {
                writer.WriteValue(value);
            }

            writer.WriteEndAttribute();
        }
        internal static void WriteElement(this XmlWriter writer, string elementName, int? value)
        {
            if (value.HasValue)
            {
                WriteElement(writer, elementName, value.Value);
            }
        }
        public static void WriteAttributeToXml([NotNull] this XmlWriter writer, [NotNull] string attributeName, int value)
        {
            writer.WriteStartAttribute(attributeName);
            writer.WriteValue(value);
            writer.WriteEndAttribute();
        }
        public static void WriteAttributeToXml([NotNull] this XmlWriter writer, [NotNull] string attributeName, bool value)
        {
            writer.WriteStartAttribute(attributeName);
            writer.WriteValue(value);
            writer.WriteEndAttribute();
        }
        public static void WriteAttributeToXml([NotNull] this XmlWriter writer, [NotNull] string attributeName, long value)
        {
            writer.WriteStartAttribute(attributeName);
            writer.WriteValue(value);
            writer.WriteEndAttribute();
        }

        public static void WriteInfo(this XmlWriter writer, string elemName, string? attribute, string? attributeVal, string? value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                writer.WriteStartElement(elemName);
                if (!string.IsNullOrEmpty(attribute) && !string.IsNullOrEmpty(attributeVal))
                {
                    writer.WriteAttributeString(attribute!, attributeVal!);
                }
                writer.WriteValue(value!);
                writer.WriteEndElement();
            }
        }

        public static void UpdateElement(this XElement root, string elementName, string? value, bool ignoreIfBlank)
        {
            if (ignoreIfBlank && value.IsNullOrWhitespace())
            {
                return;
            }
            UpdateElement(root, elementName, value!);
        }

        public static void UpdateElement([NotNull] this XElement e, [NotNull] string elementName, string? value)
        {
            if (value is null)
            {
                return;
            }
            if (e.Elements(elementName).Any())
            {
                try
                {
                    e.Elements(elementName).Single().Value = value;
                }
                catch (InvalidOperationException)
                {
                    Logger.Error($"Could not update element {elementName} in {e}");
                }
            }
            else
            {
                e.Add(new XElement(elementName, value));
            }
        }

        public static void ReplaceElements([NotNull] this XElement root, string key, [NotNull] IEnumerable<string> values)
        {
            IEnumerable<XElement> elementsToRemove = root.Elements(key).ToList();
            foreach (XElement oldValue in elementsToRemove)
            {
                oldValue.Remove();
            }

            foreach (string value in values)
            {
                root.Add(new XElement(key, value));
            }
        }

        public static void UpdateElement([NotNull] this XElement e, [NotNull] string elementName, DateTime? value)
        {
            e.SetElementValue(elementName, value);
        }

        public static void UpdateElement(this XElement e, string elementName, int value, bool ignoreIfBlank)
        {
            if (ignoreIfBlank && value == 0)
            {
                return;
            }
            e.SetElementValue(elementName, value);
        }

        public static void UpdateElement([NotNull] this XElement e, [NotNull] string elementName, float value)
        {
            e.SetElementValue(elementName, value);
        }

        public static void UpdateAttribute([NotNull] this XElement element, string attributeName, [NotNull] string value)
        {
            XAttribute? att = element.Attributes(attributeName).FirstOrDefault();
            if (att is null)
            {
                element.Add(new XAttribute(attributeName, value));
            }
            else
            {
                att.Value = value;
            }
        }

        public static void WriteInfo(this XmlWriter writer, string elemName, string? attribute, string? attributeVal)
        {
            if (string.IsNullOrEmpty(attributeVal))
            {
                return;
            }

            writer.WriteStartElement(elemName);
            if (!string.IsNullOrEmpty(attribute))
            {
                writer.WriteAttributeString(attribute!, attributeVal!);
            }
            writer.WriteEndElement();
        }

        public static void WriteInfo([NotNull] this XmlWriter writer, [NotNull] string elemName, string? attribute, bool attributeVal)
        {
            writer.WriteStartElement(elemName);
            if (!string.IsNullOrEmpty(attribute))
            {
                writer.WriteAttributeString(attribute!, XmlConvert.ToString(attributeVal));
            }
            writer.WriteEndElement();
        }

        public static bool? ExtractBool([NotNull] this XElement xmlSettings, string elementName)
        {
            if (xmlSettings.Descendants(elementName).Any())
            {
                return XmlConvert.ToBoolean((string)xmlSettings.Descendants(elementName).First());
            }

            return null;
        }

        public static bool HasAttribute([NotNull] this XElement node, string name, string value)
        {
            return node.Attributes().Any(testAttribute => testAttribute.Name == name && testAttribute.Value == value);
        }

        public static bool ExtractBool([NotNull] this XElement xmlSettings, string elementName,bool defaultValue)
        {
            if (xmlSettings.Descendants(elementName).Any())
            {
                return XmlConvert.ToBoolean((string)xmlSettings.Descendants(elementName).First());
            }

            return defaultValue;
        }
        public static bool ExtractBoolBackupDefault([NotNull] this XElement xmlSettings, string elementName, string backupElementName,bool defaultValue)
        {
            return xmlSettings.ExtractBool(elementName)
                ?? xmlSettings.ExtractBool(backupElementName)
                ?? defaultValue;
        }

        public static DateTime? ExtractDateTime([NotNull] this XElement xmlSettings, string elementName)
        {
            if (xmlSettings.Descendants(elementName).Any())
            {
                string textVersion=(string)xmlSettings.Descendants(elementName).First();
                if (string.IsNullOrWhiteSpace(textVersion))
                {
                    return null;
                }

                return XmlConvert.ToDateTime(textVersion,XmlDateTimeSerializationMode.Utc);
            }
            return null;
        }
        public static string ExtractString([NotNull] this XElement xmlSettings, string elementName)
        {
            return ExtractString(xmlSettings, elementName, string.Empty);
        }

        public static string? ExtractStringOrNull([NotNull] this XElement xmlSettings, string elementName)
        {
            if (xmlSettings.Descendants(elementName).Any())
            {
                return (string)xmlSettings.Descendants(elementName).First();
            }

            return null;
        }

        public static string ExtractString([NotNull] this XElement xmlSettings, string elementName,string defaultValue)
        {
            if (xmlSettings.Descendants(elementName).Any())
            {
                return (string)xmlSettings.Descendants(elementName).First();
            }

            return defaultValue;
        }
        public static int? ExtractInt([NotNull] this XElement xmlSettings, string elementName)
        {
            if(xmlSettings.Descendants(elementName).Any() && !string.IsNullOrWhiteSpace((string)xmlSettings.Descendants(elementName).First()))
            {
                return XmlConvert.ToInt32((string)xmlSettings.Descendants(elementName).First());
            }

            return null;
        }

        public static int ExtractInt([NotNull] this XElement xmlSettings, string elementName, int defaultValue) => ExtractInt(xmlSettings,elementName)??defaultValue;

        public static long? ExtractLong([NotNull] this XElement xmlSettings, string elementName)
        {
            if (xmlSettings.Descendants(elementName).Any())
            {
                return XmlConvert.ToInt64((string)xmlSettings.Descendants(elementName).First());
            }

            return null;
        }

        public static long ExtractLong([NotNull] this XElement xmlSettings, string elementName, int defaultValue) => ExtractLong(xmlSettings, elementName) ?? defaultValue;

        public static T ExtractEnum<T>([NotNull] this XElement xmlSettings, string elementName, T defaultVal)
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            int? val = xmlSettings.ExtractInt(elementName);

            if (val is null)
            {
                return defaultVal;
            }

            if (typeof(T).IsEnumDefined(val))
            {
                return (T)Enum.Parse(typeof(T), val.ToString(), true);
            }
            return defaultVal;
        }

        public static float ExtractFloat([NotNull] this XElement xmlSettings, string elementName, int defaultValue) => ExtractFloat(xmlSettings, elementName) ?? defaultValue;

        public static float? ExtractFloat([NotNull] this XElement xmlSettings, string elementName)
        {
            if (xmlSettings.Descendants(elementName).Any())
            {
                return XmlConvert.ToSingle((string)xmlSettings.Descendants(elementName).First());
            }

            return null;
        }

        public static double? ExtractDouble([NotNull] this XElement xmlSettings, string elementName)
        {
            if (xmlSettings.Descendants(elementName).Any())
            {
                return XmlConvert.ToDouble((string)xmlSettings.Descendants(elementName).First());
            }

            return null;
        }

        [NotNull]
        internal static List<string> ReadStringsFromXml([NotNull] this XElement rootElement, string token)
        {
            return rootElement.Descendants(token).Select(n=>n.Value.Trim()).ToList();
        }
        [NotNull]
        internal static List<IgnoreItem> ReadIiFromXml([NotNull] this XElement rootElement, string token)
        {
            return rootElement.Descendants(token).Select(n => new IgnoreItem(n.Value)).ToList();
        }

        internal static void WriteStringsToXml([NotNull] this XmlWriter writer, [NotNull] IEnumerable<IgnoreItem> ignores, [NotNull] string elementName, string stringName)
        {
            writer.WriteStartElement(elementName);
            foreach (IgnoreItem ss in ignores)
            {
                writer.WriteStartElement(stringName);
                writer.WriteValue(ss.FileAndPath);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        internal static void WriteStringsToXml([NotNull] this XmlWriter writer, [NotNull] PreviouslySeenEpisodes previouslySeenEpisodes, [NotNull] string elementName, string stringName)
        {
            writer.WriteStartElement(elementName);
            foreach (int ep in previouslySeenEpisodes)
            {
                writer.WriteStartElement(stringName);
                writer.WriteValue(ep);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
        internal static void WriteStringsToXml([NotNull] this XmlWriter writer, [NotNull] PreviouslySeenMovies previouslySeenMovies, [NotNull] string elementName, string stringName)
        {
            writer.WriteStartElement(elementName);
            foreach (int ep in previouslySeenMovies)
            {
                writer.WriteStartElement(stringName);
                writer.WriteValue(ep);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
    }
}
