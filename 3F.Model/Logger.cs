using System;
using NLog;
using _3F.Model;

namespace _3F.Log
{
    public class Logger : ILogger
    {
        public void LogInfo(string message, string action)
        {
            Log(LogLevel.Info, message, action);
        }

        public void LogWarn(string message, string action)
        {
            Log(LogLevel.Warn, message, action);
        }

        public void LogError(string message, string action)
        {
            Log(LogLevel.Error, message, action);
        }

        public void LogDebug(string message, string action)
        {
            Log(LogLevel.Debug, message, action);
        }

        public void LogFatal(string message, string action)
        {
            Log(LogLevel.Fatal, message, action);
        }

        public void LogException(Exception exception, string action)
        {
            NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
            Log(LogLevel.Error, string.Format("{0};{1}", action, exception.ToString()));
            if (exception.InnerException != null)
                Log(LogLevel.Error, string.Format("{0};{1}", action, exception.InnerException.Message));
        }

        private void Log(LogLevel level, string message, string action)
        {
            if (message.Contains("apple") && message.Contains("png")) return; //zprávy od applu nezobrazovat

            Log(level, string.Format("{0};{1}", message, action));
        }

        private void Log(LogLevel level, string message)
        {
            NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
            LogEventInfo ev = new LogEventInfo(level, logger.Name, message);
            ev.TimeStamp = Info.CentralEuropeNow;
            logger.Log(ev);
        }
    }
}
