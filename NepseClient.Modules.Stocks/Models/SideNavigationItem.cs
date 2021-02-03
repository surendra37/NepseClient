using Prism.Mvvm;

namespace NepseClient.Modules.Stocks.Models
{
    public class SideNavigationItem : BindableBase
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string _subTitle;
        public string SubTitle
        {
            get { return _subTitle; }
            set { SetProperty(ref _subTitle, value); }
        }
    }
}
