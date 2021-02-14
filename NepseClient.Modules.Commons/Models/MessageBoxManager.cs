using System;
using System.Windows;

namespace NepseClient.Modules.Commons.Models
{
    public static class MessageBoxManager
    {
        public static void ShowErrorMessage(Exception ex, string caption)
        {
            MessageBox.Show(ex.Message, caption);
        }
    }
}
