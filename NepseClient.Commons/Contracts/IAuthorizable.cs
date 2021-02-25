using RestSharp;

using System.Threading;
using System.Threading.Tasks;

namespace NepseClient.Commons.Contracts
{
    public interface IAuthorizable
    {
        IRestClient Client { get; }
        bool IsAuthenticated { get; set; }
        void Authorize();
    }

    public interface IRestAuthorizableAsync
    {
        IRestClient Client { get; }
        bool IsAuthenticated { get; set; }
        Task SignInAsync(CancellationToken ct = default);
        Task SignOutAsync(CancellationToken ct = default);
    }
}
