using Microsoft.Extensions.Logging;
using NLog;
using System;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace TVRename;

public class NlogILogger(Logger baseLogger, string details, string source) : ILogger
{
    private string Details { get; } = details;
    private string Source { get; } = source;
    private readonly Logger baseLogger = baseLogger;

    /// <exception cref="ArgumentNullException"><paramref name="formatter"/> is <see langword="null"/></exception>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        ArgumentNullException.ThrowIfNull(formatter);

        string message = $"{Source}: {formatter(state, exception)}: {Details}: {exception?.ErrorText()}";
        NLog.LogLevel convertedLogLevel = GetNLogLogLevel(logLevel);

        baseLogger.Log(convertedLogLevel, message);
    }

    public bool IsEnabled(LogLevel logLevel) => baseLogger.IsEnabled(GetNLogLogLevel(logLevel));

    private static NLog.LogLevel GetNLogLogLevel(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => NLog.LogLevel.Trace,
            LogLevel.Debug => NLog.LogLevel.Debug,
            LogLevel.Information => NLog.LogLevel.Info,
            LogLevel.Warning => NLog.LogLevel.Warn,
            LogLevel.Error => NLog.LogLevel.Error,
            LogLevel.Critical => NLog.LogLevel.Fatal,
            LogLevel.None => NLog.LogLevel.Off,
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel))
        };
    }

    /// <exception cref="ArgumentNullException"><paramref name="state"/> is <see langword="null"/></exception>
    IDisposable ILogger.BeginScope<TState>(TState state)
    {
        if (state is null)
        {
            throw new ArgumentNullException(nameof(state));
        }

        return ScopeContext.PushNestedState(state);
    }
}
