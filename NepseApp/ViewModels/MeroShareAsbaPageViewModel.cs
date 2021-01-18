using System.Windows.Controls;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace NepseApp.ViewModels
{
    public class MeroShareAsbaPageViewModel : BindableBase
    {
        private TabItem _selectedItem;
        public TabItem SelectedItem
        {
            get { return _selectedItem; }
            set { SetProperty(ref _selectedItem, value); }
        }
        public MeroShareAsbaPageViewModel(IRegionManager regionManager) 
        {
            _regionManager = regionManager;
        }

        private DelegateCommand<string> _navigateCommand;
        private readonly IRegionManager _regionManager;

        public DelegateCommand<string> NavigateCommand =>
            _navigateCommand ?? (_navigateCommand = new DelegateCommand<string>(ExecuteNavigateCommand));

        void ExecuteNavigateCommand(string source)
        {
            _regionManager.RequestNavigate("AsbaRegion", source);
        }
    }
}
