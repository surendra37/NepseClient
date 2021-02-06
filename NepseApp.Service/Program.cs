using NepseClient.Commons.Constants;

using Serilog;

using System;
using System.IO;

using Topshelf;

namespace NepseApp.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Verbose()
                .WriteTo.Console()
#endif
                .WriteTo.File(Path.Combine(PathConstants.AppDataPath.Value, "ServiceLogs", "log-.txt"),
                fileSizeLimitBytes: 4 * 1024 * 1024,
                rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
                .CreateLogger();
            var rc = HostFactory.Run(x =>
            {
                x.Service<NepseClientService>();
                x.RunAsLocalSystem();
                x.UseSerilog();

                x.SetDescription("This service will synchronize with the Nepal Stock Exchange and database");
                x.SetDisplayName("Nepse Client Service");
                x.SetServiceName("NepseService");
            });

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
            Environment.ExitCode = exitCode;
        }
    }
}
