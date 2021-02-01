using System.Collections.Generic;
using System.Linq;
using System.Windows;

using NepseApp.Views;

using NepseClient.Libraries.MeroShare;
using NepseClient.Libraries.MeroShare.Models.Responses;
using NepseClient.Modules.Commons.Models;
using NepseClient.Modules.MeroShare.Views;

using Prism.Commands;
using Prism.Mvvm;

using Serilog;

namespace NepseApp.ViewModels
{
    public class SettingsPageViewModel : BindableBase
    {
        private IEnumerable<SettingsItem> _generalItems;
        public IEnumerable<SettingsItem> GeneralItems
        {
            get { return _generalItems; }
            set { SetProperty(ref _generalItems, value); }
        }

        private IEnumerable<SettingsItem> _tmsItems;
        public IEnumerable<SettingsItem> TmsItems
        {
            get { return _tmsItems; }
            set { SetProperty(ref _tmsItems, value); }
        }

        private IEnumerable<SettingsItem> _meroShareItems;
        public IEnumerable<SettingsItem> MeroShareItems
        {
            get { return _meroShareItems; }
            set { SetProperty(ref _meroShareItems, value); }
        }

        public SettingsPageViewModel(MeroshareClient client)
        {
            var pageItems = new List<SelectedTabItem>
            {
                new SelectedTabItem { ViewName = nameof(PortfolioPage), DisplayName = "Portfolio" },
                new SelectedTabItem { ViewName = nameof(AsbaPage), DisplayName = "My ASBA" },
                new SelectedTabItem { ViewName = nameof(TmsLiveMarketPage), DisplayName = "Live Market" },
                new SelectedTabItem { ViewName = nameof(SettingsPage), DisplayName = "Settings" },
            };
            GeneralItems = new List<SettingsItem>
            {
                new ToggleSettingsItem
                {
                    IconKind = MaterialDesignThemes.Wpf.PackIconKind.Refresh,
                    Name = "Auto Refresh",
                    IsChecked = Settings.Default.AutoRefreshOnLoad,
                    OnSave = value => Settings.Default.AutoRefreshOnLoad = value,
                    OnReset = () => Settings.Default.AutoRefreshOnLoad,
                },

                new ComboBoxSettingsItem
                {
                    IconKind = MaterialDesignThemes.Wpf.PackIconKind.Tab,
                    Name = "Open Tab on Load",
                    DisplayMemberPath = "DisplayName",
                    Items = pageItems,
                    SelectedItem = pageItems.FirstOrDefault(x => x.ViewName.Equals(Settings.Default.SelectedTab)),
                    OnSave = value => Settings.Default.SelectedTab = (value as SelectedTabItem)?.ViewName,
                    OnReset = () => pageItems.FirstOrDefault(x => x.ViewName.Equals(Settings.Default.SelectedTab)),
                }

            };

            TmsItems = new List<SettingsItem>
            {
                new SettingsHeaderItem
                {
                    Name = "Tms",
                },

                new TextBoxSettingsItem
                {
                    IconKind = MaterialDesignThemes.Wpf.PackIconKind.Web,
                    Name = "Base URL",
                    Value = Settings.Default.TmsBaseUrl,
                    OnSave = value => Settings.Default.TmsBaseUrl = value,
                    OnReset = () => Settings.Default.TmsBaseUrl,
                }
            };

            var capitals = /*new MeroshareCapitalResponse[0];//*/ client.GetCapitals();
            MeroShareItems = new List<SettingsItem>
            {
                new SettingsHeaderItem
                {
                    Name = "MeroShare",
                },

                new ComboBoxSettingsItem
                {
                    IconKind = MaterialDesignThemes.Wpf.PackIconKind.Tab,
                    Name = "DP",
                    Items = capitals,
                    SelectedItem = capitals.FirstOrDefault(x => x.Id.Equals(Settings.Default.MeroShareClientId)),
                    OnSave = value => Settings.Default.MeroShareClientId = (value as MeroshareCapitalResponse)?.Id,
                    OnReset = () => capitals.FirstOrDefault(x => x.Id.Equals(Settings.Default.MeroShareClientId)),
                }
            };
        }

        private DelegateCommand _saveCommand;
        public DelegateCommand SaveCommand =>
            _saveCommand ?? (_saveCommand = new DelegateCommand(ExecuteSaveCommand));

        void ExecuteSaveCommand()
        {
            try
            {
                foreach (var item in GeneralItems)
                {
                    item.Save();
                }
                foreach (var item in TmsItems)
                {
                    item.Save();
                }
                foreach (var item in MeroShareItems)
                {
                    item.Save();
                }
                Settings.Default.Save();
                //ReloadCommand.Execute();
                MessageBox.Show("Settings is saved", "Settings saved", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (System.Exception ex)
            {
                Log.Warning(ex, "Failed to reset settings");
                MessageBox.Show("There was some error while resetting", "Failed to reset", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private DelegateCommand _resetCommand;
        public DelegateCommand ResetCommand =>
            _resetCommand ?? (_resetCommand = new DelegateCommand(ExecuteResetCommand));

        void ExecuteResetCommand()
        {
            try
            {
                Settings.Default.Reset();
                foreach (var item in GeneralItems)
                {
                    item.Reset();
                }
                foreach (var item in TmsItems)
                {
                    item.Reset();
                }
                foreach (var item in MeroShareItems)
                {
                    item.Reset();
                }
                //ReloadCommand.Execute();                
                MessageBox.Show("Settings is resetted", "Settings resetted", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (System.Exception ex)
            {
                Log.Warning(ex, "Failed to reset settings");
                MessageBox.Show("There was some error while resetting", "Failed to reset", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
