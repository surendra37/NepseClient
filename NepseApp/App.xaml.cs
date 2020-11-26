using NepseApp.Models;
using NepseApp.ViewModels;
using NepseApp.Views;
using NepseClient.Commons;
using NepseClient.Commons.Contracts;
using Prism.Ioc;
using Serilog;
using Serilog.Formatting.Compact;

using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

using TradeManagementSystem.Nepse;
using TradeManagementSystemClient;

namespace NepseApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var logPath = Path.Combine(Constants.AppDataPath.Value, "Logs", "log-.jl");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
#if DEBUG
                .WriteTo.Console()
#endif
                .WriteTo.File(new CompactJsonFormatter(), logPath, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Debug("Starting application");

            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            base.OnStartup(e);
        }

        private void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Fatal(e.Exception, "Unknown error occured");
            var result = MessageBox.Show("An unknown error has occured. Do you wish to continue? \n" + e.Exception.Message, 
                "Unknown Error", MessageBoxButton.YesNo, MessageBoxImage.Error);

            e.Handled = result == MessageBoxResult.Yes;
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IApplicationCommand, ApplicationCommand>();

            var meroshareClient = new MeroshareClient();
            meroshareClient.RestoreSession();
            containerRegistry.RegisterInstance(meroshareClient);

            var nepseClient = new TmsClient();
            nepseClient.RestoreSession();
            containerRegistry.RegisterInstance<INepseClient>(nepseClient);

            var socketHelper = new SocketHelper(nepseClient);
            containerRegistry.RegisterInstance(socketHelper);

            containerRegistry.RegisterDialog<AuthenticationDialog, AuthenticationDialogViewModel>();
            containerRegistry.RegisterDialog<MeroshareImportDialog, MeroshareImportDialogViewModel>();

            containerRegistry.RegisterForNavigation<PortfolioPage, PortfolioPageViewModel>();
            containerRegistry.RegisterForNavigation<LiveMarketPage, LiveMarketPageViewModel>();
            containerRegistry.RegisterForNavigation<DashboardPage, DashboardPageViewModel>();
            containerRegistry.RegisterForNavigation<MarketDepthPage, MarketDepthPageViewModel>();
            containerRegistry.RegisterForNavigation<SettingsPage, SettingsPageViewModel>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Container.Resolve<INepseClient>().SaveSession();
            Container.Resolve<MeroshareClient>().SaveSession();
            Container.Resolve<SocketHelper>().Stop();
            base.OnExit(e);
        }
    }
}
