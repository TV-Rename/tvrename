using NLog;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace TVRename.Settings.AppState
{
    public class State
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public UpdateCheck UpdateCheck { get; } = new UpdateCheck();

        public static State LoadFromDefaultFile() => LoadFromFile(PathManager.StateFile.FullName);

        public static State LoadFromFile(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    XmlSerializer? serializer = new XmlSerializer(typeof(State));
                    using (XmlReader? reader = XmlReader.Create(path))
                    {
                        return (State)serializer.Deserialize(reader);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "Could not load app state file {0}", path);
                    return new State();
                }
            }

            return new State();
        }

        public void SaveToDefaultFile()
        {
            SaveToFile(PathManager.StateFile.FullName);
        }

        public void SaveToFile(string path)
        {
            XmlSerializer? serializer = new XmlSerializer(typeof(State));
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings { Indent = true };
            using (XmlWriter? xmlWriter = XmlWriter.Create(path, xmlWriterSettings))
            {
                serializer.Serialize(xmlWriter, this);
            }
        }
    }
}
