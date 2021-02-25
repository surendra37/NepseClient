using MaterialDesignExtensions.Controls;

using System.Windows.Controls;

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
            var menus = new MenuItem[] { TmsMenu, MeroShareMenu, StockMenu };
            foreach (var menu in menus)
            {
                if (menu is not null && sender is not null)
                {
                    menu.IsChecked = menu == sender;
                }
            }
        }
    }
}
