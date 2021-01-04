using System;

namespace NepseService.TradeManagementSystem
{
    public static class Log
    {
        public static ILogger Logger { get; set; }
        public static void Debug(string message, params object[] parameters) => Logger?.Debug(message, parameters);
        public static void Error(string message, params object[] parameters) => Logger?.Debug(message, parameters);
        public static void Fatal(string message, params object[] parameters) => Logger?.Debug(message, parameters);
        public static void Information(string message, params object[] parameters) => Logger?.Debug(message, parameters);
        public static void Verbose(string message, params object[] parameters) => Logger?.Debug(message, parameters);
        public static void Warning(string message, params object[] parameters) => Logger?.Debug(message, parameters);

        public static void Debug(Exception ex, string message, params object[] parameters) { }
        public static void Error(Exception ex, string message, params object[] parameters) { }
        public static void Fatal(Exception ex, string message, params object[] parameters) { }
        public static void Information(Exception ex, string message, params object[] parameters) { }
        public static void Verbose(Exception ex, string message, params object[] parameters) { }
        public static void Warning(Exception ex, string message, params object[] parameters) { }
    }
}
