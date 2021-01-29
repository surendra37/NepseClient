using MaterialDesignExtensions.Controls;
using Prism.Services.Dialogs;

namespace NepseClient.Modules.Commons.Views
{
    /// <summary>
    /// Interaction logic for CustomDialogWindow.xaml
    /// </summary>
    public partial class CustomDialogWindow : MaterialWindow, IDialogWindow
    {
        /// <summary>
        /// The <see cref="IDialogResult"/> of the dialog.
        /// </summary>
        public IDialogResult Result { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomDialogWindow"/> class.
        /// </summary>
        public CustomDialogWindow()
        {
            InitializeComponent();
        }
    }
}
