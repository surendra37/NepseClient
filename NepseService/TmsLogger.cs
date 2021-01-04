using NepseService.TradeManagementSystem;

using Log = Serilog.Log;
namespace NepseService
{

    public class TmsLogger : ILogger
    {
        public void Debug(string message, params object[] parameters) => Log.Debug(message, parameters);

        public void Error(string message, params object[] parameters) => Log.Error(message, parameters);

        public void Fatal(string message, params object[] parameters) => Log.Fatal(message, parameters);

        public void Information(string message, params object[] parameters) => Log.Information(message, parameters);

        public void Verbose(string message, params object[] parameters) => Log.Verbose(message, parameters);

        public void Warning(string message, params object[] parameters) => Log.Warning(message, parameters);
    }
}