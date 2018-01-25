using System;
using System.Xml;

namespace TVRename.Core.Extensions
{
    public static class XmlExtensions
    {
        public static void WriteNode(this XmlWriter writer, string key, string value)
        {
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
    }
}
