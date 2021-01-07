
using System.Windows.Controls;

using NepseApp.Models;

using Serilog;

namespace NepseApp.ViewModels
{
    public class MeroShareAsbaPageViewModel : ActiveAwareBindableBase
    {
        private TabItem _selectedItem;
        public TabItem SelectedItem
        {
            get { return _selectedItem; }
            set { SetProperty(ref _selectedItem, value); }
        }
        public MeroShareAsbaPageViewModel(IApplicationCommand appCommand) : base(appCommand)
        {
        }

        public override void ExecuteRefreshCommand()
        {
            if (SelectedItem is null || 
                SelectedItem.Content is not ContentControl view ||
                view.DataContext is not ActiveAwareBindableBase vm) return;
            try
            {
                IsBusy = vm.IsBusy;
                vm.ExecuteRefreshCommand();
                IsBusy = vm.IsBusy;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Unknown error");
            }
        }
    }
}
