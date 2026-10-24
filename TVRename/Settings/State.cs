//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//


namespace TVRename.Settings;

public class State
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private State() { }

    private State(DateTime? d)
    {
        LastUpdateCheckUtc = d;
    }

    private DateTime? LastUpdateCheckUtc { get; set; }

    public TimeSpan LastUpdate => TimeHelpers.UtcNow() - LastUpdateCheckUtc.GetValueOrDefault(DateTime.MinValue);

    internal async Task UpdateLastCheckTime()
    {
        LastUpdateCheckUtc = TimeHelpers.UtcNow();
        await SaveToFile(PathManager.StateFile.FullName);
    }

    public static async Task<State> LoadFromDefaultFileAsync() => await LoadFromFile(PathManager.StateFile.FullName);

    private static async Task<State> LoadFromFile(string path)
    {
        if (File.Exists(path))
        {
            try
            { 
                await using var stream = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read, bufferSize: 4096, useAsync: true);
                XElement x = await XElement.LoadAsync(stream, LoadOptions.None,CancellationToken.None);
                DateTime? d = x.ExtractDateTime("LastUpdateCheckUtc");
                return new State(d);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, $"Could not load app state file {path}");
            }
        }
        else
        {
            Logger.Warn($"Could not access {path} - no state.xml will be loaded");
        }

        return new State();
    }

    private async Task SaveToFile(string path)
    {
        await XmlHelper.SaveXmlFileAsync(GenerateXML(), path);
    }

    private XDocument GenerateXML()
    {
        return new XDocument(
           new XDeclaration("1.0", "utf-8", "yes"), // XML Declaration
           GenerateXElement());
    }

    internal XElement GenerateXElement()
    {
        return new XElement("State",
            new XElement("LastUpdateCheckUtc", LastUpdateCheckUtc)
        );
    }
}
