using Serilog;
using System.Runtime.CompilerServices;
using System.IO;

namespace AlarmCompanyManager.Utilities
{
    public static class Logger
    {
        private static ILogger? _logger;

        public static void Initialize()
        {
            _logger = new LoggerConfiguration()
                .WriteTo.File("logs/alarm-company-manager-.txt",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            _logger.Information("Alarm Company Manager application started");
        }

        public static void LogInfo(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFile = "")
        {
            _logger?.Information("{CallerFile}::{CallerName} - {Message}",
                Path.GetFileNameWithoutExtension(callerFile), callerName, message);
        }

        public static void LogWarning(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFile = "")
        {
            _logger?.Warning("{CallerFile}::{CallerName} - {Message}",
                Path.GetFileNameWithoutExtension(callerFile), callerName, message);
        }

        public static void LogError(Exception exception, string message = "", [CallerMemberName] string callerName = "", [CallerFilePath] string callerFile = "")
        {
            _logger?.Error(exception, "{CallerFile}::{CallerName} - {Message}",
                Path.GetFileNameWithoutExtension(callerFile), callerName, message);
        }

        public static void LogDebug(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFile = "")
        {
            _logger?.Debug("{CallerFile}::{CallerName} - {Message}",
                Path.GetFileNameWithoutExtension(callerFile), callerName, message);
        }

        public static void Shutdown()
        {
            _logger?.Information("Alarm Company Manager application shutting down");
            Log.CloseAndFlush();
        }
    }
}