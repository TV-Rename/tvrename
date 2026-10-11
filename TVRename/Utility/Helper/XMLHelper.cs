//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using CefSharp.DevTools.CSS;
using SharpCompress.Writers;
using System.IO;

namespace TVRename;

public static class XmlHelper
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static async Task SaveXmlFileAsync(XDocument doc, string filePath, CancellationToken cancellationToken = default)
    {
        // 1. Configure the writer to support async behavior
        XmlWriterSettings settings = new XmlWriterSettings
        {
            Async = true,
            Indent = true, // Optional: formats the XML output
            NewLineOnAttributes = true
        };

        // 2. Open the file stream and the writer asynchronously
        await using FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true);
        await using XmlWriter xmlWriter = XmlWriter.Create(stream, settings);

        // 3. Save the document
        await doc.SaveAsync(xmlWriter, cancellationToken).ConfigureAwait(false);
    }

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

    public static async Task<XDocument> LoadXmlFromFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        // Open an async-compatible file stream
        using FileStream stream = System.IO.File.OpenRead(filePath);
        // Load the document asynchronously
        XDocument doc = await XDocument.LoadAsync(stream, LoadOptions.None, cancellationToken);
        return doc;
    }

    public static string ReadStringFixQuotesAndSpaces(string s)
    {
        string res = s.Replace("\\'", "'");
        res = res.Replace("\\\"", "\"");
        res = res.Trim();
        return res;
    }
    public static string ValidXml(this string? content) => string.IsNullOrEmpty(content) ? string.Empty : new([.. content.Where(XmlConvert.IsXmlChar)]);

    public static void WriteElement(this XmlWriter writer, string elementName, string? value)
    {
        WriteElement(writer, elementName, value, false);
    }

    /// <summary>
    /// Saves an XDocument to a file asynchronously.
    /// </summary>
    /// <param name="doc">The XDocument instance to save.</param>
    /// <param name="filePath">The destination file path.</param>
    /// <param name="cancellationToken">Optional token to cancel the operation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public static async Task SaveXmlAsync(this XDocument doc, string filePath, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(doc);
        ArgumentException.ThrowIfNullOrWhiteSpace("filePath");

        // Use FileOptions.Asynchronous to ensure true non-blocking OS I/O
        using FileStream stream = new(
            filePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 4096,
            options: FileOptions.Asynchronous);
        await doc.SaveAsync(stream, SaveOptions.None, cancellationToken);
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
            Logger.Error($"Cound not write {elementName} with value {value} because {e.ErrorText()}");
        }

        writer.WriteEndElement();
    }

    public static void WriteElement(this XmlWriter writer, string elementName, double value)
    {
        WriteElement(writer, elementName, value, null, null);
    }

    public static XElement? GenerateDoubleXElement(string elementName, double? value, string? format, IFormatProvider? provider)
    {
        if (!value.HasValue)
        {
            return null;
        }
        if (format is null || provider is null)
        {
            return new XElement(elementName, value);
        }
        else
        {
            return new XElement(elementName, value?.ToString(format, provider));
        }
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

    public static XElement GenerateXmlElementsFromCollection<T>(string elementName, IEnumerable<T> collection, Func<T, XElement> elementGenerator)
    {
        XElement root = new(elementName);
        foreach (T item in collection)
        {
            root.Add(elementGenerator(item));
        }
        return root;
    }


    public static XElement GenerateStringsXMLElement(IEnumerable<string> strings, string elementName, string stringName)
    {
        XElement root = new(elementName);
        foreach (string s in strings)
        {
            root.Add(new XElement(stringName, s));
        }
        return root;
    }
    public static XElement GenerateIntsXMLElement(IEnumerable<int> strings, string elementName, string stringName)
    {
        XElement root = new(elementName);
        foreach (int s in strings)
        {
            root.Add(new XElement(stringName, s));
        }
        return root;
    }

    public static XElement GetOrCreateElement(this XElement root, string elementName)
    {
        if (root.Elements(elementName).IsAny())
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
        else if (!e.Elements(elementName).IsAny())
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
        IEnumerable<XElement> elementsToRemove = [.. root.Elements(key)];
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


    internal static XElement? GenerateInfoXElement(string elemName, string? attribute, string? attributeVal)
    {
        if (string.IsNullOrEmpty(attributeVal))
        {
            return null;
        }

        var element = new XElement(elemName);
        if (!string.IsNullOrEmpty(attribute))
        {
            element.Add(new XAttribute(attribute, attributeVal));
        }
        return element;
    }

    public static XElement? GenerateInfoXElement(string elemName, string? attribute, bool attributeVal)
    {
        var element = new XElement(elemName);
        if (!string.IsNullOrEmpty(attribute))
        {
            element.Add(new XAttribute(attribute, XmlConvert.ToString(attributeVal)));
        }
        return element;
    }

    public static bool? ExtractBool(this XElement xmlSettings, string elementName)
    {
        if (xmlSettings.Descendants(elementName).IsAny())
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
        if (xmlSettings.Descendants(elementName).IsAny())
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
        if (xmlSettings.Descendants(elementName).IsAny())
        {
            string textVersion = (string)xmlSettings.Descendants(elementName).First();
            if (string.IsNullOrWhiteSpace(textVersion))
            {
                return null;
            }

            try
            {
                return XmlConvert.ToDateTime(textVersion, XmlDateTimeSerializationMode.Utc);
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }
        return null;
    }

    public static string ExtractString(this XElement xmlSettings, string elementName)
    {
        return ExtractString(xmlSettings, elementName, string.Empty);
    }

    public static string? ExtractStringOrNull(this XElement xmlSettings, string elementName)
    {
        if (xmlSettings.Descendants(elementName).IsAny())
        {
            return (string)xmlSettings.Descendants(elementName).First();
        }

        return null;
    }

    public static string ExtractString(this XElement xmlSettings, string elementName, string defaultValue)
    {
        if (xmlSettings.Descendants(elementName).IsAny())
        {
            return (string)xmlSettings.Descendants(elementName).First();
        }

        return defaultValue;
    }

    private static T? ExtractNumber<T>(this XElement xmlSettings, string elementName, Func<string, T> functionToExtract) where T : struct
    {
        IEnumerable<XElement> xElements = [.. xmlSettings.Descendants(elementName)];

        if (xElements.IsAny() && !string.IsNullOrWhiteSpace((string)xElements.First()))
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

    /// <exception cref="ArgumentException">T must be an enumerated type</exception>
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
            try
            {
                return (T)Enum.Parse(typeof(T), val.Value.ToString(), true);
            }
            catch (OverflowException)
            {
                return defaultVal;
            }
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
        return [.. rootElement.Descendants(token).Select(n => n.Value.Trim())];
    }

    internal static List<IgnoreItem> ReadIiFromXml(this XElement rootElement, string token)
    {
        return [.. rootElement.Descendants(token).Select(n => new IgnoreItem(n.Value))];
    }

    internal static XElement? GenerateOptionalXElement(string elementName, string? value) => GenerateXElement(elementName, value, true);

    private static XElement? GenerateXElement(string elementName, string? value, bool ignoreIfBlank)
    {
        if (ignoreIfBlank && string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return new XElement(elementName, value.ValidXml());
    }

    internal static XElement? GenerateOptionalXElement(string elementName, int? value, bool ignoreZero = true)
    {
        if (value is null)
        {
            return null;
        }
        if (ignoreZero && value <= 0)
        {
            return null;
        }
        return new XElement(elementName, value);
    }

    internal static XElement? GenerateOptionalXElement(string elementName, DateTime? value)
    {
        if (value == null)
        {
            return null;
        }  
        return new XElement(elementName, value);
    }
}
