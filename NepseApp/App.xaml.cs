using Prism.Ioc;
using NepseApp.Views;
using System.Windows;
using NepseClient.Commons;
using TradeManagementSystemClient;
using NepseApp.ViewModels;
using Serilog;

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

            Log.Debug("Starting application");
            base.OnStartup(e);
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var nepseClient = new TmsClient("https://tms49.nepsetms.com.np/");
            nepseClient.RestoreSession();
            containerRegistry.RegisterInstance<INepseClient>(nepseClient);

            containerRegistry.RegisterForNavigation<PortfolioPage, PortfolioPageViewModel>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Container.Resolve<INepseClient>().SaveSession();
            base.OnExit(e);
        }
    }
}
