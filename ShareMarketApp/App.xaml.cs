using System.Windows;

using Microsoft.Extensions.Configuration;

using NepseService.TradeManagementSystem;

using Prism.Ioc;

using ShareMarketApp.ViewModels;
using ShareMarketApp.Views;

namespace ShareMarketApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            containerRegistry.RegisterInstance<IConfiguration>(config);
            containerRegistry.RegisterSingleton<TmsClient>();
            containerRegistry.RegisterSingleton<MeroshareClient>();

            containerRegistry.RegisterForNavigation<MerosharePortfolioPage, MerosharePortfolioPageViewModel>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Container.Resolve<TmsClient>().Dispose();
            Container.Resolve<MeroshareClient>().Dispose();
            base.OnExit(e);
        }
    }
}
