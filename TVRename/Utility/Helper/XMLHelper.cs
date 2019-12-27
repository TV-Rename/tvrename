using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace TVRename
{
    public static class XmlHelper
    {
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

        public static void WriteElement(this XmlWriter writer, string elementName, [CanBeNull] string value)
        {
            WriteElement(writer, elementName, value, false);
        }

        public static void WriteElement(this XmlWriter writer, string elementName, [CanBeNull] string value,bool ignoreIfBlank)
        {
            if (ignoreIfBlank && string.IsNullOrEmpty(value))
            {
                return;
            }

            writer.WriteStartElement(elementName);
            writer.WriteValue(value??string.Empty);
            writer.WriteEndElement();
        }

        public static void WriteElement([NotNull] this XmlWriter writer, [NotNull] string elementName, double value)
        {
            WriteElement(writer, elementName, value, null);
        }

        public static void WriteElement([NotNull] this XmlWriter writer, [NotNull] string elementName, double value,[CanBeNull] string format)
        {
            writer.WriteStartElement(elementName);
            if (format is null)
            {
                writer.WriteValue(value);
            }
            else
            {
                writer.WriteValue(value.ToString(format));
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

        public static void WriteInfo(this XmlWriter writer, string elemName, [CanBeNull] string attribute, [CanBeNull] string attributeVal, [CanBeNull] string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                writer.WriteStartElement(elemName);
                if (!string.IsNullOrEmpty(attribute) && !string.IsNullOrEmpty(attributeVal))
                {
                    writer.WriteAttributeString(attribute, attributeVal);
                }
                writer.WriteValue(value);
                writer.WriteEndElement();
            }
        }

        public static void WriteInfo(this XmlWriter writer, string elemName, [CanBeNull] string attribute, [CanBeNull] string attributeVal)
        {
            if (!string.IsNullOrEmpty(attributeVal))
            {
                writer.WriteStartElement(elemName);
                if (!string.IsNullOrEmpty(attribute))
                {
                    writer.WriteAttributeString(attribute, attributeVal);
                }
                writer.WriteEndElement();
            }
        }

        public static bool? ExtractBool([NotNull] this XElement xmlSettings, string elementName)
        {
            if (xmlSettings.Descendants(elementName).Any())
            {
                return XmlConvert.ToBoolean((string)xmlSettings.Descendants(elementName).First());
            }

            return null;
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

        [NotNull]
        internal static List<string> ReadStringsFromXml([NotNull] this XElement rootElement, string token)
        {
            return rootElement.Descendants(token).Select(n=>n.Value).ToList();
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
    }
}
