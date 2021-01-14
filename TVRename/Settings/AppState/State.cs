using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using TVRename;
using TVRename.Settings;
using TVRename.Settings.AppState;

namespace TVRename.Settings.AppState
{
    public class State
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public UpdateCheck UpdateCheck { get; set; } = new UpdateCheck();

        public static State LoadFromDefaultFile()
        {
            return LoadFromFile(PathManager.StateFile.FullName);
        }

        public static State LoadFromFile(string path)
        {

            if (File.Exists(path))
            {
                try
                {
                    var serializer = new XmlSerializer(typeof(State));
                    using (var reader = XmlReader.Create(path))
                    {
                        return (State)serializer.Deserialize(reader);
                    }
                }
                catch(Exception ex)
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
            var serializer = new XmlSerializer(typeof(State));
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Indent = true;
            using (var xmlWriter = System.Xml.XmlWriter.Create(path, xmlWriterSettings))
            {
                serializer.Serialize(xmlWriter, this);
            }
        }
    }
}
