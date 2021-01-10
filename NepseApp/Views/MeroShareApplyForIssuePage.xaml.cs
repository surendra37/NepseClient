using System.Windows;
using System.Windows.Controls;

namespace NepseApp.Views
{
    /// <summary>
    /// Interaction logic for MeroShareApplyForIssuePage
    /// </summary>
    public partial class MeroShareApplyForIssuePage : UserControl
    {
        public MeroShareApplyForIssuePage()
        {
            InitializeComponent();
        }

        private void DG_Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Documents.Hyperlink link = (System.Windows.Documents.Hyperlink)e.OriginalSource;
            //Process.Start(link.NavigateUri.AbsoluteUri);
        }
    }
}
