using System;
using System.Collections.Generic;
using System.Xml;

namespace TVRename.Core.Extensions
{
    public static class XmlExtensions
    {
        public static void WriteNode(this XmlWriter writer, string key, string value, bool skipEmpty = false)
        {
            if (skipEmpty && string.IsNullOrWhiteSpace(value)) return;

            writer.WriteStartElement(key);
            writer.WriteValue(value ?? string.Empty);
            writer.WriteEndElement();
        }

        public static void WriteNode(this XmlWriter writer, string key, bool value)
        {
            writer.WriteStartElement(key);
            writer.WriteValue(value);
            writer.WriteEndElement();
        }

        public static void WriteNode(this XmlWriter writer, string key, int value)
        {
            writer.WriteStartElement(key);
            writer.WriteValue(value);
            writer.WriteEndElement();
        }

        public static void WriteNode(this XmlWriter writer, string key, decimal value)
        {
            writer.WriteStartElement(key);
            writer.WriteValue(value);
            writer.WriteEndElement();
        }

        public static void WriteNode(this XmlWriter writer, string key, DateTime value)
        {
            writer.WriteStartElement(key);
            writer.WriteValue(value);
            writer.WriteEndElement();
        }

        public static void WriteNodes(this XmlWriter writer, string key, IEnumerable<string> values)
        {
            foreach (string value in values)
            {
                writer.WriteNode(key, value, true);
            }
        }
    }
}
