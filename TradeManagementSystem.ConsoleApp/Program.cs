using System;
using TradeManagementSystem.ConsoleApp.Models;
using TradeManagementSystemClient;
using TradeManagementSystemClient.Models.Requests;
using Serilog;
using ConsoleTables;

namespace TradeManagementSystem.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                Log.Debug("Starting application");
                var client = new TmsClient(new Configuration())
                {
                    PromptCredentials = GetCredentials
                };
                //using (var socketHelper = new SocketHelper(client))
                //{
                //    socketHelper.Send("index", true);

                //    while (!socketHelper.Received)
                //    {
                //        socketHelper.SendOpCode();
                //        System.Threading.Thread.Sleep(10000);
                //    }
                //}
                var securities = client.GetSecurities();
                ConsoleTable
                    .From(securities.Payload.Data)
                    .Write();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unknown error");
            }

            Log.Debug("Application ended.");
        }

        private static AuthenticationRequest GetCredentials()
        {
            Log.Debug("Getting credentials");
            var config = new Configuration().Tms;
            var output = new AuthenticationRequest(config.Username, config.Password);
            return output;
        }
    }
}
