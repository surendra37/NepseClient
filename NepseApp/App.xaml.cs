using NepseApp.Models;
using NepseApp.ViewModels;
using NepseApp.Views;

using NepseClient.Commons.Constants;
using NepseClient.Commons.Contracts;
using NepseClient.Modules.Commons.Adapters;
using NepseClient.Modules.Commons.Interfaces;
using NepseClient.Modules.Commons.Models;
using NepseClient.Modules.Commons.Views;
using NepseClient.Modules.MeroShare;
using NepseClient.Modules.TradeManagementSystem;

using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

using Serilog;
using Serilog.Formatting.Compact;

using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

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
            var logPath = Path.Combine(PathConstants.AppDataPath.Value, "Logs", "log-.jl");
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
            containerRegistry.RegisterDialogWindow<CustomDialogWindow>();

            containerRegistry.RegisterSingleton<IApplicationCommand, ApplicationCommand>();
            containerRegistry.RegisterSingleton<IConfiguration, Configuration>();
            containerRegistry.RegisterSingleton<MeroshareClient>();
            containerRegistry.RegisterSingleton<TmsClient>();

            containerRegistry.RegisterDialog<AuthenticationDialog, AuthenticationDialogViewModel>();

            
            containerRegistry.RegisterForNavigation<SettingsPage, SettingsPageViewModel>();
            containerRegistry.RegisterForNavigation<TmsLiveMarketPage, TmsLiveMarketPageViewModel>();
            
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);

            moduleCatalog.AddModule<MeroShareModule>();
            moduleCatalog.AddModule<TradeManagementSystemModule>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Container.Resolve<TmsClient>().Dispose();
            Container.Resolve<MeroshareClient>().Dispose();
            Log.CloseAndFlush();
            base.OnExit(e);
        }
        protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings)
        {
            base.ConfigureRegionAdapterMappings(regionAdapterMappings);
            // old Mapping == Prism.Regions.SelectorRegionAdapter
            //regionAdapterMappings.RegisterMapping(typeof(TabControl), Container.Resolve<TabControlAdapter>());
        }
    }
}
