using NepseApp.Models;
using NepseApp.ViewModels;
using NepseApp.Views;

using NepseClient.Commons.Constants;
using NepseClient.Commons.Contracts;
using NepseClient.Modules.Commons.Interfaces;
using NepseClient.Modules.Commons.Views;

using Ookii.Dialogs.Wpf;

using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

using Serilog;
using Serilog.Formatting.Compact;

using System.IO;
using System.Windows;
using System.Windows.Threading;

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
#if DEBUG
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                #else
                .MinimumLevel.Debug()
#endif
                .WriteTo.File(new CompactJsonFormatter(), logPath, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Debug("Starting application");

            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            base.OnStartup(e);

            var regionManager = Container.Resolve<IRegionManager>();
            //regionManager.RegisterViewWithRegion(RegionNames.SideNavRegion, typeof(NepseClient.Modules.Stocks.Views.SideNavPage));
            regionManager.RegisterViewWithRegion(RegionNames.SideNavRegion, typeof(NepseClient.Modules.TradeManagementSystem.Views.SideNavPage));
        }

        private void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Fatal(e.Exception, "Unknown error occured");
            if (!TaskDialog.OSSupportsTaskDialogs)
            {
                var result = MessageBox.Show("Do you want to ignore this unknown error?\n\r" + e.Exception.Message,
                    "Unknown error occured", MessageBoxButton.YesNo, MessageBoxImage.Error);
                e.Handled = result == MessageBoxResult.Yes;
                return;
            }
            using (var dialog = new TaskDialog())
            {
                dialog.WindowTitle = "Unknown error occured";
                dialog.MainInstruction = "An unknown error has occurred. Do you wish to ignore and continue?";
                dialog.Content = e.Exception.Message;
                dialog.ExpandedInformation = e.Exception.StackTrace;
                dialog.MainIcon = TaskDialogIcon.Error;
                TaskDialogButton okButton = new TaskDialogButton("Ignore");
                TaskDialogButton cancelButton = new TaskDialogButton("Exit");
                dialog.Buttons.Add(okButton);
                dialog.Buttons.Add(cancelButton);
                TaskDialogButton button = dialog.ShowDialog(Current.MainWindow);

                e.Handled = button == okButton;
            };
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
            containerRegistry.RegisterSingleton<NepseClient.Libraries.MeroShare.MeroshareClient>();
            containerRegistry.RegisterSingleton<NepseClient.Libraries.TradeManagementSystem.TmsClient>();
            containerRegistry.RegisterSingleton<NepseClient.Libraries.NepalStockExchange.Contexts.DatabaseContext, SQLiteDatabaseContext>();

            containerRegistry.RegisterDialog<AuthenticationDialog, AuthenticationDialogViewModel>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);

            moduleCatalog.AddModule<NepseClient.Modules.Stocks.StocksModule>();
            moduleCatalog.AddModule<NepseClient.Modules.MeroShare.MeroShareModule>();
            moduleCatalog.AddModule<NepseClient.Modules.TradeManagementSystem.TradeManagementSystemModule>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Container.Resolve<NepseClient.Libraries.TradeManagementSystem.TmsClient>().SignOut();
            Container.Resolve<NepseClient.Libraries.MeroShare.MeroshareClient>().SignOut();
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
