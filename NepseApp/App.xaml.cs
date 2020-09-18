using Prism.Ioc;
using NepseApp.Views;
using System.Windows;
using NepseClient.Commons;
using TradeManagementSystemClient;
using NepseApp.ViewModels;
using Serilog;
using Prism.Services.Dialogs;
using System.Security.Authentication;

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
            var nepseClient = new ProxyNepseClient(new TmsClient(), ShowAuthDialog);
            nepseClient.RestoreSession();
            containerRegistry.RegisterInstance<INepseClient>(nepseClient);

            containerRegistry.RegisterDialog<AuthenticationDialog, AuthenticationDialogViewModel>();

            containerRegistry.RegisterForNavigation<PortfolioPage, PortfolioPageViewModel>();
        }

        private void ShowAuthDialog(INepseClient client)
        {
            var dialog = Container.Resolve<IDialogService>();
            var success = false;
            dialog.ShowDialog(nameof(AuthenticationDialog), new DialogParameters { { "Client", client } }, result =>
            {
                success = result?.Result == ButtonResult.OK;
            });
            if (!success)
                throw new AuthenticationException("Not Authenticated.");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Container.Resolve<INepseClient>().SaveSession();
            base.OnExit(e);
        }
    }
}
