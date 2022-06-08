using System;

namespace _3F.Log
{
    public interface ILogger
    {
        void LogInfo(string message, string action);

        void LogWarn(string message, string action);

        void LogError(string message, string action);

        void LogDebug(string message, string action);

        void LogFatal(string message, string action);

        void LogException(Exception exception, string action);
    }
}
