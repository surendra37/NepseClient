using MaterialDesignThemes.Wpf;
using Prism.Commands;
using System;

namespace NepseClient.Modules.Commons.Interfaces
{
    public interface IApplicationCommand
    {
        CompositeCommand RefreshCommand { get; }
        ISnackbarMessageQueue MessageQueue { get; }
        bool RefreshOnActive { get; set; }
        Action<string> ShowMessage { get; set; }
    }
}