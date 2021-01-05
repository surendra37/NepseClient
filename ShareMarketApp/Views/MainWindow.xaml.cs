using System.Diagnostics;
using System.Windows;

using MaterialDesignExtensions.Controls;
using MaterialDesignExtensions.Model;

namespace ShareMarketApp.Views
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
            SelectNavigationItem(args.NavigationItem);
        }

        private void SelectNavigationItem(INavigationItem navigationItem)
        {
            if (navigationItem != null)
            {
                object newContent = navigationItem.NavigationItemSelectedCallback(navigationItem);

                //if (contentControl.Content == null || contentControl.Content.GetType() != newContent.GetType())
                //{
                //    contentControl.Content = newContent;
                //}
            }
            //else
            //{
            //    contentControl.Content = null;
            //}

            if (appBar != null)
            {
                appBar.IsNavigationDrawerOpen = false;
            }
        }

        private void GoToGitHubButtonClickHandler(object sender, RoutedEventArgs args)
        {
            OpenLink("https://github.com/spiegelp/MaterialDesignExtensions");
        }

        private void GoToDocumentation(object sender, RoutedEventArgs args)
        {

            //if (contentControl.Content is ViewModel.ViewModel viewModel && !string.IsNullOrWhiteSpace(viewModel.DocumentationUrl))
            //{
            //    OpenLink(viewModel.DocumentationUrl);
            //}
        }

        private void OpenLink(string url)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            };

            Process.Start(psi);
        }
    }
}
