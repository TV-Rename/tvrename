using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace TVRename;

public static class XmlHelper
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static void WriteStringsToXml(IEnumerable<string> strings, XmlWriter writer, string elementName, string stringName)
    {
        writer.WriteStartElement(elementName);
        writer.WriteStringsToXml(stringName, strings);
        writer.WriteEndElement();
    }

    public static void WriteStringsToXml(this XmlWriter writer, string elementName, IEnumerable<string> strings)
    {
        foreach (string ss in strings)
        {
            writer.WriteStartElement(elementName);
            writer.WriteValue(ss.ValidXml());
            writer.WriteEndElement();
        }
    }

    public static string ReadStringFixQuotesAndSpaces(string s)
    {
        string res = s.Replace("\\'", "'");
        res = res.Replace("\\\"", "\"");
        res = res.Trim();
        return res;
    }
    public static string ValidXml(this string content) => new(content.Where(XmlConvert.IsXmlChar).ToArray());

    public static void WriteElement(this XmlWriter writer, string elementName, string? value)
    {
        WriteElement(writer, elementName, value, false);
    }

    public static void WriteElement(this XmlWriter writer, string elementName, string? value, bool ignoreIfBlank)
    {
        if (ignoreIfBlank && string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        writer.WriteStartElement(elementName);
        try
        {
            writer.WriteValue(value?.ValidXml() ?? string.Empty);
        }
        catch (ArgumentException e)
        {
            Logger.Error($"Cound not write {elementName} with value {value} because {e.Message}");
        }

        writer.WriteEndElement();
    }

    public static void WriteElement(this XmlWriter writer, string elementName, double value)
    {
        WriteElement(writer, elementName, value, null, null);
    }

    public static void WriteElement(this XmlWriter writer, string elementName, double? value, string? format, IFormatProvider? provider)
    {
        if (!value.HasValue)
        {
            return;
        }
        writer.WriteStartElement(elementName);
        if (format is null || provider is null)
        {
            writer.WriteValue(value);
        }
        else
        {
            writer.WriteValue(value.Value.ToString(format, provider));
        }
        writer.WriteEndElement();
    }

    public static void WriteElement(this XmlWriter writer, string elementName, int value)
    {
        WriteElement(writer, elementName, value, false);
    }

    public static void WriteElement(this XmlWriter writer, string elementName, int value, bool ignoreZero)
    {
        if (ignoreZero && value == 0)
        {
            return;
        }

        writer.WriteStartElement(elementName);
        writer.WriteValue(value);
        writer.WriteEndElement();
    }

    public static void WriteElement(this XmlWriter writer, string elementName, bool value)
    {
        writer.WriteStartElement(elementName);
        writer.WriteValue(value);
        writer.WriteEndElement();
    }

    public static void WriteElement(this XmlWriter writer, string elementName, DateTime? value)
    {
        writer.WriteStartElement(elementName);
        if (value != null)
        {
            writer.WriteValue(value);
        }

        writer.WriteEndElement();
    }

    public static void WriteAttributeToXml(this XmlWriter writer, string attributeName, string value)
    {
        writer.WriteStartAttribute(attributeName);
        writer.WriteValue(value.ValidXml());
        writer.WriteEndAttribute();
    }

    public static XElement GetOrCreateElement(this XElement root, string elementName)
    {
        if (root.Elements(elementName).Any())
        {
            return root.Elements(elementName).First();
        }
        XElement e = new(elementName);
        root.Add(e);
        return e;
    }

    public static XElement GetOrCreateElement(this XElement root, string elementName, string name, string value)
    {
        if (root.Elements(elementName).Any(el => el.HasAttribute(name, value)))
        {
            return root.Elements(elementName).First(el => el.HasAttribute(name, value));
        }
        XElement e = new(elementName);
        root.Add(e);
        return e;
    }

    public static void WriteAttributeToXml(this XmlWriter writer, string attributeName, DateTime? value)
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

    public static void WriteAttributeToXml(this XmlWriter writer, string attributeName, int value)
    {
        writer.WriteStartAttribute(attributeName);
        writer.WriteValue(value);
        writer.WriteEndAttribute();
    }

    public static void WriteAttributeToXml(this XmlWriter writer, string attributeName, bool value)
    {
        writer.WriteStartAttribute(attributeName);
        writer.WriteValue(value);
        writer.WriteEndAttribute();
    }

    public static void WriteAttributeToXml(this XmlWriter writer, string attributeName, long value)
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
                writer.WriteAttributeString(attribute, attributeVal);
            }
            writer.WriteValue(value.ValidXml());
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

    public static void UpdateElement(this XElement e, string elementName, string? value)
    {
        if (value is null)
        {
            return;
        }
        if (e.Elements(elementName).Count() == 1)
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
        else if (!e.Elements(elementName).Any())
        {
            e.Add(new XElement(elementName, value));
        }
        else
        {
            ReplaceElements(e, elementName, value.AsList());
        }
    }

    public static void ReplaceElements(this XElement root, string key, IEnumerable<string> values)
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

    public static void UpdateElement(this XElement e, string elementName, DateTime? value)
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

    public static void UpdateElement(this XElement e, string elementName, float value)
    {
        e.SetElementValue(elementName, value);
    }

    public static void UpdateAttribute(this XElement element, string attributeName, string value)
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
            writer.WriteAttributeString(attribute, attributeVal);
        }
        writer.WriteEndElement();
    }

    public static void WriteInfo(this XmlWriter writer, string elemName, string? attribute, bool attributeVal)
    {
        writer.WriteStartElement(elemName);
        if (!string.IsNullOrEmpty(attribute))
        {
            writer.WriteAttributeString(attribute, XmlConvert.ToString(attributeVal));
        }
        writer.WriteEndElement();
    }

    public static bool? ExtractBool(this XElement xmlSettings, string elementName)
    {
        if (xmlSettings.Descendants(elementName).Any())
        {
            return XmlConvert.ToBoolean((string)xmlSettings.Descendants(elementName).First());
        }

        return null;
    }

    public static bool HasAttribute(this XElement node, string name, string value)
    {
        return node.Attributes().Any(testAttribute => testAttribute.Name == name && testAttribute.Value == value);
    }

    public static bool ExtractBool(this XElement xmlSettings, string elementName, bool defaultValue)
    {
        if (xmlSettings.Descendants(elementName).Any())
        {
            return XmlConvert.ToBoolean((string)xmlSettings.Descendants(elementName).First());
        }

        return defaultValue;
    }

    public static bool ExtractBoolBackupDefault(this XElement xmlSettings, string elementName, string backupElementName, bool defaultValue)
    {
        return xmlSettings.ExtractBool(elementName)
               ?? xmlSettings.ExtractBool(backupElementName)
               ?? defaultValue;
    }

    public static DateTime? ExtractDateTime(this XElement xmlSettings, string elementName)
    {
        if (xmlSettings.Descendants(elementName).Any())
        {
            string textVersion = (string)xmlSettings.Descendants(elementName).First();
            if (string.IsNullOrWhiteSpace(textVersion))
            {
                return null;
            }

            return XmlConvert.ToDateTime(textVersion, XmlDateTimeSerializationMode.Utc);
        }
        return null;
    }

    public static string ExtractString(this XElement xmlSettings, string elementName)
    {
        return ExtractString(xmlSettings, elementName, string.Empty);
    }

    public static string? ExtractStringOrNull(this XElement xmlSettings, string elementName)
    {
        if (xmlSettings.Descendants(elementName).Any())
        {
            return (string)xmlSettings.Descendants(elementName).First();
        }

        return null;
    }

    public static string ExtractString(this XElement xmlSettings, string elementName, string defaultValue)
    {
        if (xmlSettings.Descendants(elementName).Any())
        {
            return (string)xmlSettings.Descendants(elementName).First();
        }

        return defaultValue;
    }

    private static T? ExtractNumber<T>(this XElement xmlSettings, string elementName, Func<string, T> functionToExtract) where T : struct
    {
        IEnumerable<XElement> xElements = xmlSettings.Descendants(elementName).ToList();

        if (xElements.Any() && !string.IsNullOrWhiteSpace((string)xElements.First()))
        {
            try
            {
                return functionToExtract((string)xElements.First());
            }
            catch (FormatException)
            {
                Logger.Error($"Could not parse '{elementName}' from {xmlSettings}");
                return null;
            }
        }

        return null;
    }

    public static int? ExtractInt(this XElement xmlSettings, string elementName)
    {
        return ExtractNumber(xmlSettings, elementName, XmlConvert.ToInt32);
    }

    public static int ExtractInt(this XElement xmlSettings, string elementName, int defaultValue) => ExtractInt(xmlSettings, elementName) ?? defaultValue;

    public static long? ExtractLong(this XElement xmlSettings, string elementName)
    {
        return ExtractNumber(xmlSettings, elementName, XmlConvert.ToInt64);
    }

    public static long ExtractLong(this XElement xmlSettings, string elementName, int defaultValue) => ExtractLong(xmlSettings, elementName) ?? defaultValue;

    public static T? ExtractEnum<T>(this XElement xmlSettings, string elementName, T? defaultVal)
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
            return (T)Enum.Parse(typeof(T), val.Value.ToString(), true);
        }
        return defaultVal;
    }

    public static float ExtractFloat(this XElement xmlSettings, string elementName, int defaultValue) => ExtractFloat(xmlSettings, elementName) ?? defaultValue;

    public static float? ExtractFloat(this XElement xmlSettings, string elementName)
    {
        return ExtractNumber(xmlSettings, elementName, XmlConvert.ToSingle);
    }

    public static double? ExtractDouble(this XElement xmlSettings, string elementName)
    {
        return ExtractNumber(xmlSettings, elementName, XmlConvert.ToDouble);
    }

    internal static List<string> ReadStringsFromXml(this XElement rootElement, string token)
    {
        return rootElement.Descendants(token).Select(n => n.Value.Trim()).ToList();
    }

    internal static List<IgnoreItem> ReadIiFromXml(this XElement rootElement, string token)
    {
        return rootElement.Descendants(token).Select(n => new IgnoreItem(n.Value)).ToList();
    }

    internal static void WriteStringsToXml(this XmlWriter writer, IEnumerable<IgnoreItem> ignores, string elementName, string stringName)
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

    internal static void WriteStringsToXml(this XmlWriter writer, PreviouslySeenEpisodes previouslySeenEpisodes, string elementName, string stringName)
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

    internal static void WriteStringsToXml(this XmlWriter writer, PreviouslySeenMovies previouslySeenMovies, string elementName, string stringName)
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
