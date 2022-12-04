using Alphaleonis.Win32.Filesystem;
using NLog;
using System;
using System.Xml;
using System.Xml.Serialization;

namespace TVRename.Settings.AppState;

public class State
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public UpdateCheck UpdateCheck { get; } = new();

    public static State LoadFromDefaultFile() => LoadFromFile(PathManager.StateFile.FullName);

    private static State LoadFromFile(string path)
    {
        if (File.Exists(path))
        {
            try
            {
                XmlSerializer serializer = new(typeof(State));
                using XmlReader reader = XmlReader.Create(path);
                State? s = serializer.Deserialize(reader) as State;
                return s ?? new State();
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, $"Could not load app state file {path}");
                return new State();
            }
        }

        return new State();
    }

    public void SaveToDefaultFile()
    {
        SaveToFile(PathManager.StateFile.FullName);
    }

    private void SaveToFile(string path)
    {
        XmlSerializer serializer = new(typeof(State));
        XmlWriterSettings xmlWriterSettings = new() { Indent = true };
        using XmlWriter xmlWriter = XmlWriter.Create(path, xmlWriterSettings);
        serializer.Serialize(xmlWriter, this);
    }
}
