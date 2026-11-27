//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//
namespace TVRename.Settings;

using Polly;
using Polly.Retry;
using System.Web.Services.Description;
using TMDbLib.Objects.Authentication;
using TMDbLib.Objects.Exceptions;
using TMDbLib.Objects.Movies;

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

    internal async Task UpdateLastCheckTime(CancellationToken token)
    {
        LastUpdateCheckUtc = TimeHelpers.UtcNow();
        await SaveToFile(PathManager.StateFile.FullName, token);
    }

    public static async Task<State> LoadFromDefaultFileAsync() => await LoadFromFile(PathManager.StateFile.FullName);

    private static async Task<State> LoadFromFile(string path)
    {
        if (File.Exists(path))
        {
            try
            {
                await using var stream = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read, bufferSize: 4096, useAsync: true);
                XElement x = await XElement.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
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

    private async Task SaveToFile(string path, CancellationToken token)
    {
        var pipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<Exception>(),
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(2),
                BackoffType = DelayBackoffType.Linear,
                UseJitter = true,
                OnRetry = args =>
                {
                    Logger.Warn(args.Outcome.Exception, $"Retry {args.AttemptNumber}/3 to save {path}.");
                    return ValueTask.CompletedTask;
                }
            })
            .AddTimeout(TimeSpan.FromSeconds(20))
            .Build();

        try
        {
            await pipeline.ExecuteAsync(async (token) => await XmlHelper.SaveXmlFileAsync(GenerateXML(), path, token), token);
        }
        catch (Exception e)
        {
            if (Environment.UserInteractive)
            {
                Logger.Error($"Complete failure to save {path}. {e.Message}");
                DialogResult dr = Forms.ThemeableMessageBox.Show($"Could not save {path} due to {e.Message}. State will be lost. Retry? ", "TV Rename",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (dr != DialogResult.No)
                {
                    await SaveToFile(path, token);
                }
            }
        }
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
