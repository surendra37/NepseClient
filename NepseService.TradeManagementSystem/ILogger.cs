namespace NepseService.TradeManagementSystem
{
    public interface ILogger
    {
        void Verbose(string message, params object[] parameters);
        void Debug(string message, params object[] parameters);
        void Information(string message, params object[] parameters);
        void Warning(string message, params object[] parameters);
        void Error(string message, params object[] parameters);
        void Fatal(string message, params object[] parameters);
    }
}