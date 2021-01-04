using System.Linq;

using ConsoleTables;

using Microsoft.Extensions.Configuration;

using NepseService.TradeManagementSystem;

using Serilog;

using Log = Serilog.Log;

namespace NepseService
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();



            TradeManagementSystem.Log.Logger = new TmsLogger();

            try
            {
                //TestTms(configuration);
                TestMeroshare(configuration);
            }
            catch (System.Exception ex)
            {
                Log.Fatal(ex, "Unknown error");
            }
        }

        private static void TestMeroshare(IConfiguration config)
        {
            var host = config["Meroshare:Host"];
            var clientId = config["Meroshare:ClientId"];
            var username = config["Meroshare:Username"];
            var password = config["Meroshare:Password"];
            var demat = config["meroshare:Demat"];

            using var client = new MeroshareClient(host, clientId, username, password);

            var myShares = client.GetMyShares();
            ConsoleTable.From(myShares.Select(x => new { Scip = x }))
                .Write();

            var purchases = client.ViewMyPurchase(demat, myShares).ToArray();
            ConsoleTable.From(purchases)
                .Write();

            var searches = client.SearchMyPurchase(demat, myShares).ToArray();
            ConsoleTable.From(searches.SelectMany(x => x).Select(x => new { x.Scrip, x.TransactionQuantity, x.UserPrice, x.PurchaseSource }))
                .Write();
        }

        private static void TestTms(IConfiguration configuration)
        {
            var host = configuration["Tms:Host"];
            var username = configuration["Tms:Username"];
            var password = configuration["Tms:Password"];
            var clientId = configuration["Tms:ClientId"];

            using var client = new TmsClient(host, username, password);
            var myPortfolios = client.GetMyPortfolio(clientId);
            ConsoleTable.From(myPortfolios)
                .Write();
        }
    }
}
