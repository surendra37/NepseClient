using MaterialDesignExtensions.Model;

namespace NepseClient.Modules.Commons.Interfaces
{
    public interface IViewNavigationItem : INavigationItem
    {
        string Target { get; }
    }
}
