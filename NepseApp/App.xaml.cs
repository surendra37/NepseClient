using NepseApp.Models;
using NepseApp.ViewModels;
using NepseApp.Views;
using NepseClient.Commons;
using NepseClient.Commons.Contracts;
using Prism.Ioc;
using Prism.Services.Dialogs;
using Serilog;
using System.Security.Authentication;
using System.Windows;
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
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            ConfigUtils.LoadSettings();

            Log.Debug("Starting application");
            base.OnStartup(e);
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IApplicationCommand, ApplicationCommand>();

            var nepseClient = new TmsClient();
            nepseClient.RestoreSession();
            containerRegistry.RegisterInstance<INepseClient>(nepseClient);

            var socketHelper = new SocketHelper(nepseClient);
            containerRegistry.RegisterInstance(socketHelper);

            containerRegistry.RegisterDialog<AuthenticationDialog, AuthenticationDialogViewModel>();

            containerRegistry.RegisterForNavigation<PortfolioPage, PortfolioPageViewModel>();
            containerRegistry.RegisterForNavigation<LiveMarketPage, LiveMarketPageViewModel>();
            containerRegistry.RegisterForNavigation<DashboardPage, DashboardPageViewModel>();
            containerRegistry.RegisterForNavigation<MarketDepthPage, MarketDepthPageViewModel>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Container.Resolve<INepseClient>().SaveSession();
            Container.Resolve<SocketHelper>().Stop();
            base.OnExit(e);
        }
    }
}
