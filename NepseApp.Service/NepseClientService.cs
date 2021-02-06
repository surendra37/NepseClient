using NepseClient.Libraries.NepalStockExchange;
using NepseClient.Libraries.NepalStockExchange.Contexts;

using Serilog;

using System;
using System.Timers;

using Topshelf;
using Topshelf.Hosts;

namespace NepseApp.Service
{
    public class NepseClientService : ServiceControl
    {
        readonly DatabaseContext _database;
        readonly ServiceClient _service;
        readonly Timer _timer;
        public NepseClientService()
        {
            _service = new ServiceClient();
            _database = new SQLiteDatabaseContext();
            _timer = new Timer(60_000) { AutoReset = true };
            _timer.Elapsed += TimerElapsed;
            TimerElapsed(this, null);
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Log.Debug("Update price started");
                var requireUpdate = true;
                var lastUpdatedOn = _database.TodayPrice.GetLastUpdatedOn();
                // Check for valid date
                var minDate = DateTime.Today.AddHours(15);
                var maxDate = DateTime.Today.AddHours(11);
                if (DateTime.Today.DayOfWeek == DayOfWeek.Friday)
                {
                    minDate = minDate.AddDays(-1);
                    maxDate = maxDate.AddDays(2);
                }
                else if (DateTime.Today.DayOfWeek == DayOfWeek.Saturday)
                {
                    minDate = minDate.AddDays(-2);
                    maxDate = maxDate.AddDays(1);
                }
                if (lastUpdatedOn >= minDate && lastUpdatedOn <= maxDate)
                {
                    // no update
                    requireUpdate = false;
                }

                if (requireUpdate)
                {
                    Log.Debug("Updating prices...");
                    var prices = _service.GetTodaysPriceAll();
                    _database.TodayPrice.Clear();
                    _database.TodayPrice.AddRange(prices);
                    _database.TodayPrice.SetLastUpdated();
                }
                else
                {
                    Log.Debug("No update required");
                    Log.Debug("Last updated on: {0}", lastUpdatedOn);
                }

                Log.Debug("Next updated on {0}", DateTime.Now.AddMilliseconds(_timer.Interval));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to update database");
            }
        }

        public bool IsConsole { get; set; }

        public bool Start(HostControl hostControl)
        {
            try
            {
                IsConsole = hostControl is ConsoleRunHost;

                _timer.Start();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to start service");
                return false;
            }
        }

        public bool Stop(HostControl hostControl)
        {
            try
            {
                _timer.Stop();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to stop service");
                return false;
            }
        }
    }
}
