using System;
using Serilog;
using TradeManagementSystemClient;
using NepseConsoleClient.Demos;

namespace NepseConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Verbose()
                .CreateLogger();

            Log.Verbose("Application started");
            var baseUrl = args[0];
            var username = args[1];
            var password = args[2];
            if (string.IsNullOrEmpty(baseUrl))
            {
                Log.Information("Base Url is empty. Closing application");
                return;
            }

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                Log.Information("Username/Password is empty. Closing application");
                return;
            }

            try
            {
                Log.Verbose("Creating rest client");
                var client = new TmsClient();

                var businessDate = client.GetBusinessDate();
                Log.Debug("Business Date: {0}", businessDate);

                client.RestoreSession();
                var demo = new TmsDemo(client);
                demo.Run(username, password);
                client.SaveSession();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Unknown error");
            }
        }
    }
}
