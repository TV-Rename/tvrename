using Microsoft.Extensions.Logging;
using NLog;
using System;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace TVRename;

public class NlogILogger : ILogger
{
    private readonly Logger baseLogger;

    public NlogILogger(Logger baseLogger)
    {
        this.baseLogger = baseLogger;
    }

    /// <exception cref="ArgumentNullException"><paramref name="formatter"/> is <see langword="null"/></exception>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (formatter == null)
        {
            throw new ArgumentNullException(nameof(formatter));
        }

        string message = formatter(state, exception);
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
    public IDisposable BeginScope<TState>(TState state)
    {
        if (state is null)
        {
            throw new ArgumentNullException(nameof(state));
        }

        return ScopeContext.PushNestedState(state);
    }
}
