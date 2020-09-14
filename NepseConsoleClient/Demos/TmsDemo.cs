using System;
using NepseClient.Commons;
using System.Collections.Generic;
using ConsoleTables;
using System.Linq;
using Serilog;
namespace NepseConsoleClient.Demos
{
    public class TmsDemo
    {
        private readonly INepseClient _client;

        public TmsDemo(INepseClient client)
        {
            _client = client;
        }

        public void Run(string username, string password)
        {
            Log.Information("Running demo");
            var demos = GetDemos(username, password)
                .ToDictionary(x => x.Id);

            var table = new ConsoleTable("ID", "Description");
            table.Options.EnableCount = false;
            foreach (var demo in demos)
            {
                table.AddRow(demo.Key, demo.Value.Description);
            }
            table.AddRow("Q/q", "Exit");
            
            while (true)
            {
                table.Write(Format.Minimal);
                while (true)
                {
                    Console.Write("Your Selection: ");
                    var selection = Console.ReadLine();
                    // Handle exit condition
                    if (selection.Equals("q", StringComparison.OrdinalIgnoreCase))
                    {
                        return;
                    }
                    if (!demos.ContainsKey(selection))
                    {
                        Log.Warning("Invalid Selection.");
                        continue;
                    }
                    try
                    {
                        demos[selection].Command?.Invoke();
                        Log.Information("Execution complete.");
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Failed to execute command");
                    }
                    break;
                }
            }
        }

        private IEnumerable<DemoBase> GetDemos(string username, string password)
        {
            return new List<DemoBase>
            {
                new DemoBase
                {
                    Id = "a",
                    Description = "Authenticate",
                    Command = () => _client.Authenticate(username, password)
                },new DemoBase
                {
                    Id = "l",
                    Description = "Log out",
                    Command = _client.Logout
                },
                new DemoBase
                {
                    Id = "p",
                    Description = "Portfolio",
                    Command  = () =>
                    {
                        var portfolio = _client.GetMyPortfolio();
                        ConsoleTable.From(portfolio)
                        .Write();
                    }
                },
            };
        }
    }
}
