using NLog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TVRename;

public static class LoggerHelper
{
    public static ILogger AsILogger(this Logger baseLogger) => new NlogILogger(baseLogger);
}
