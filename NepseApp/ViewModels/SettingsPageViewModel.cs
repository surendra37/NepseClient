using System.Collections.Generic;
using System.Windows;

using NepseApp.Models;

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

        public SettingsPageViewModel()
        {
            GeneralItems = new List<SettingsItem>
            {
                new ToggleSettingsItem
                {
                    IconKind = MaterialDesignThemes.Wpf.PackIconKind.Refresh,
                    Name = "Auto Refresh",
                    IsChecked = Settings.Default.AutoRefreshOnLoad,
                    OnSave = isChecked => Settings.Default.AutoRefreshOnLoad = isChecked,
                    OnReset = () => Settings.Default.AutoRefreshOnLoad,
                },
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
