using System;
using System.Collections.Generic;
using UnityEngine;
using ILogger = RG.EscapeRoomServer.Server.ILogger;

public class UnityTestLogger : ILogger
{
    private readonly Queue<LogMessage> messagesToLog = new Queue<LogMessage>();
    public void Info(string message)
    {
        messagesToLog.Enqueue(new LogMessage(message, null,Level.Info));
        OutputLog();
    }

    public void Error(string error, Exception exception)
    {
        messagesToLog.Enqueue(new LogMessage(error, exception,Level.Error));
        OutputLog();
    }

    public void OutputLog()
    {
        while (messagesToLog.Count > 0)
        {
            var logMessage = messagesToLog.Dequeue();
            switch (logMessage.level)
            {
                case Level.Info:
                    Debug.Log(logMessage.message);
                    break;
                case Level.Error:
                    Debug.LogError(logMessage.message);
                    Debug.LogException(logMessage.exception);
                    break;
            }
        }
    }
    internal class LogMessage
    {
        public string message;
        public Exception exception;
        public Level level;

        public LogMessage(string message, Exception exception, Level level)
        {
            this.message = message;
            this.exception = exception;
            this.level = level;
        }
    }
    internal enum Level
    {
        Info,
        Error
    }

}
