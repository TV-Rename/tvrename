using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace TVRename
{
    public static class XmlHelper
    {
        public static void WriteStringsToXml(IEnumerable<string> strings, XmlWriter writer, string elementName, string stringName)
        {
            writer.WriteStartElement(elementName);
            foreach (string ss in strings)
            {
                writer.WriteStartElement(stringName);
                writer.WriteValue(ss);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        public static string ReadStringFixQuotesAndSpaces(string s)
        {
            string res = s.Replace("\\'", "'");
            res = res.Replace("\\\"", "\"");
            res = res.Trim();
            return res;
        }

        public static void WriteElementToXml(XmlWriter writer, string elementName, string value,bool ignoreifBlank = false)
        {
            if (ignoreifBlank && String.IsNullOrEmpty(value)) return;

            writer.WriteStartElement(elementName);
            writer.WriteValue(value??"");
            writer.WriteEndElement();
        }
        public static void WriteElementToXml(XmlWriter writer, string elementName, double value,string format= null)
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
        public static void WriteElementToXml(XmlWriter writer, string elementName, int value,bool ignoreZero=false)
        {
            if (ignoreZero && value == 0) return;
            writer.WriteStartElement(elementName);
            writer.WriteValue(value);
            writer.WriteEndElement();
        }
        public static void WriteElementToXml(XmlWriter writer, string elementName, bool value)
        {
            writer.WriteStartElement(elementName);
            writer.WriteValue(value);
            writer.WriteEndElement();
        }
        public static void WriteElementToXml(XmlWriter writer, string elementName, DateTime? value)
        {
            writer.WriteStartElement(elementName);
            if (value != null)
                writer.WriteValue(value);
            writer.WriteEndElement();
        }

        public static void WriteAttributeToXml(XmlWriter writer, string attributeName, string value)
        {
            writer.WriteStartAttribute(attributeName);
            writer.WriteValue(value);
            writer.WriteEndAttribute();
        }
        public static void WriteAttributeToXml(XmlWriter writer, string attributeName, DateTime?  value)
        {
            writer.WriteStartAttribute(attributeName);
            if (value != null)
                writer.WriteValue(value);
            writer.WriteEndAttribute();
        }
        internal static void WriteElementToXml(XmlWriter writer, string attributeName, int? value)
        {
            if (value.HasValue) WriteElementToXml(writer,attributeName,value.Value);
        }
        public static void WriteAttributeToXml(XmlWriter writer, string attributeName, int value)
        {
            writer.WriteStartAttribute(attributeName);
            writer.WriteValue(value);
            writer.WriteEndAttribute();
        }
        public static void WriteAttributeToXml(XmlWriter writer, string attributeName, bool value)
        {
            writer.WriteStartAttribute(attributeName);
            writer.WriteValue(value);
            writer.WriteEndAttribute();
        }
        public static void WriteAttributeToXml(XmlWriter writer, string attributeName, long value)
        {
            writer.WriteStartAttribute(attributeName);
            writer.WriteValue(value);
            writer.WriteEndAttribute();
        }

        public static void WriteInfo(XmlWriter writer, string elemName, string attribute, string attributeVal, string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                writer.WriteStartElement(elemName);
                if (!String.IsNullOrEmpty(attribute) && !String.IsNullOrEmpty(attributeVal))
                {
                    writer.WriteAttributeString(attribute, attributeVal);
                }
                writer.WriteValue(value);
                writer.WriteEndElement();
            }
        }

        public static void WriteInfo(XmlWriter writer, string elemName, string attribute, string attributeVal)
        {
            if (!String.IsNullOrEmpty(attributeVal))
            {
                writer.WriteStartElement(elemName);
                if (!String.IsNullOrEmpty(attribute) && !String.IsNullOrEmpty(attributeVal))
                {
                    writer.WriteAttributeString(attribute, attributeVal);
                }
                writer.WriteEndElement();
            }
        }

        public static bool? ExtractBool(this XElement xmlSettings, string elementName)
        {
            if (xmlSettings.Descendants(elementName).Any())
                return XmlConvert.ToBoolean((string)(xmlSettings.Descendants(elementName).First()));

            return null;
        }

        public static bool ExtractBool(this XElement xmlSettings, string elementName,bool defaultValue)
        {
            if (xmlSettings.Descendants(elementName).Any())
                return XmlConvert.ToBoolean((string)(xmlSettings.Descendants(elementName).First()));

            return defaultValue;
        }
        public static bool ExtractBoolBackupDefault(this XElement xmlSettings, string elementName, string backupElementName,bool defaultValue)
        {
            return xmlSettings.ExtractBool(elementName)
                ?? xmlSettings.ExtractBool(backupElementName)
                ?? defaultValue;
        }

        public static DateTime? ExtractDateTime(this XElement xmlSettings, string elementName)
        {
            if (xmlSettings.Descendants(elementName).Any())
            {
                string textVersion=(string)(xmlSettings.Descendants(elementName).First());
                if (String.IsNullOrWhiteSpace(textVersion)) return null;
                return XmlConvert.ToDateTime(textVersion,XmlDateTimeSerializationMode.Utc);
            }
            return null;
        }
        public static string ExtractString(this XElement xmlSettings, string elementName)
        {
            return ExtractString(xmlSettings, elementName, String.Empty);
        }
        public static string ExtractString(this XElement xmlSettings, string elementName,string defaultValue)
        {
            if (xmlSettings.Descendants(elementName).Any())
                return (string)(xmlSettings.Descendants(elementName).First());

            return defaultValue;
        }
        public static int? ExtractInt(this XElement xmlSettings, string elementName)
        {
            if(xmlSettings.Descendants(elementName).Any() && !String.IsNullOrWhiteSpace((string)(xmlSettings.Descendants(elementName).First())))
                return XmlConvert.ToInt32((string)(xmlSettings.Descendants(elementName).First()));

            return null;
        }

        public static long? ExtractLong(this XElement xmlSettings, string elementName)
        {
            if (xmlSettings.Descendants(elementName).Any())
                return XmlConvert.ToInt64((string)(xmlSettings.Descendants(elementName).First()));

            return null;
        }

        public static T ExtractEnum<T>(this XElement xmlSettings, string elementName, T defaultVal)
        {
            if (!typeof(T).IsEnum) throw new ArgumentException("T must be an enumerated type");

            int? val = xmlSettings.ExtractInt(elementName);

            if (val == null) return defaultVal;

            if (typeof(T).IsEnumDefined(val))
            {
                return (T)Enum.Parse(typeof(T), val.ToString(), true);
            }
            return defaultVal;
        }

        public static float? ExtractFloat(this XElement xmlSettings, string elementName)
        {
            if (xmlSettings.Descendants(elementName).Any())
                return XmlConvert.ToSingle((string)(xmlSettings.Descendants(elementName).First()));

            return null;
        }

        internal static List<string> ReadStringsFromXml(this XElement rootElement, string token)
        {
            return rootElement.Descendants(token).Select(n=>n.Value).ToList();
        }
        internal static List<IgnoreItem> ReadIiFromXml(this XElement rootElement, string token)
        {
            return rootElement.Descendants(token).Select(n => new IgnoreItem(n.Value)).ToList();
        }

        internal static void WriteStringsToXml(IEnumerable<IgnoreItem> ignores, XmlWriter writer, string elementName, string stringName)
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

        internal static void WriteStringsToXml(PreviouslySeenEpisodes previouslySeenEpisodes, XmlWriter writer, string elementName, string stringName)
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
