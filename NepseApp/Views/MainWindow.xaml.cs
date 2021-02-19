using MaterialDesignExtensions.Controls;

namespace NepseApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MaterialWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void NavigationItemSelectedHandler(object sender, NavigationItemSelectedEventArgs args)
        {
            if (args.NavigationItem is null) return;
            // do navigation according to the selected item in args.NavigationItem
            args.NavigationItem.NavigationItemSelectedCallback?.Invoke(args.NavigationItem);
        }

        private void MenuItem_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender == StockMenu)
            {
                //StockMenu.IsChecked = false;
                TmsMenu.IsChecked = false;
                MeroShareMenu.IsChecked = false;
            }
            else if (sender == TmsMenu)
            {
                StockMenu.IsChecked = false;
                //TmsMenu.IsChecked = false;
                MeroShareMenu.IsChecked = false;
            }
            else if (sender == MeroShareMenu)
            {
                StockMenu.IsChecked = false;
                TmsMenu.IsChecked = false;
                //MeroShareMenu.IsChecked = false;
            }
        }
    }
}
