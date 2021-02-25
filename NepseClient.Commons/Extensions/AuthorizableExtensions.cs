using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using NepseClient.Commons.Contracts;

using RestSharp;

namespace NepseClient.Commons.Extensions
{
    public static class AuthorizableExtensions
    {
        public static IRestResponse<T> AuthorizedGet<T>(this IAuthorizable authorizable, IRestRequest request)
        {
            return AuthorizedMethods<T>(authorizable, request, Method.GET);
        }

        public static IRestResponse<T> AuthorizedPost<T>(this IAuthorizable authorizable, IRestRequest request)
        {
            return AuthorizedMethods<T>(authorizable, request, Method.POST);
        }

        private static IRestResponse<T> AuthorizedMethods<T>(IAuthorizable authorizable, IRestRequest request, Method method)
        {
            if (!authorizable.IsAuthenticated)
            {
                ClearAuthHeader(request);
                authorizable.Authorize();
            }

            var response = GetResponse<T>(authorizable.Client, request, method);
            if (response.IsSuccessful) return response;

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                ClearAuthHeader(request);
                authorizable.IsAuthenticated = false;
                authorizable.Authorize();
                response = GetResponse<T>(authorizable.Client, request, method);
            }

            return response;
        }

        private static void ClearAuthHeader(IRestRequest request)
        {
            var authHeader = request.Parameters.FirstOrDefault(x => x.Name.Equals("Authorization"));
            if (authHeader is null) return;

            request.Parameters.Remove(authHeader);
        }

        private static IRestResponse<T> GetResponse<T>(IRestClient client, IRestRequest request, Method method)
        {
            return method switch
            {
                Method.GET => client.Get<T>(request),
                Method.POST => client.Post<T>(request),
                Method.PUT => client.Put<T>(request),
                Method.DELETE => client.Delete<T>(request),
                Method.HEAD => client.Head<T>(request),
                Method.OPTIONS => client.Options<T>(request),
                Method.PATCH => client.Patch<T>(request),
                Method.MERGE => throw new Exception("Merge Not supported"),
                Method.COPY => throw new Exception("Copy Not supported"),
                _ => client.Get<T>(request),
            };
        }


        public static Task<T> AuthorizeGetAsync<T>(this IRestAuthorizableAsync service, IRestRequest request, CancellationToken ct)
        {
            return AuthorizeAsync<T>(service, request, Method.GET, ct);
        }
        public static Task<T> AuthorizePostAsync<T>(this IRestAuthorizableAsync service, IRestRequest request, CancellationToken ct)
        {
            return AuthorizeAsync<T>(service, request, Method.POST, ct);
        }
        public static async Task<T> AuthorizeAsync<T>(this IRestAuthorizableAsync service, IRestRequest request, Method method, CancellationToken ct)
        {
            if (!service.IsAuthenticated)
            {
                await service.SignInAsync(ct);
            }

            var response = await service.Client.ExecuteAsync<T>(request, method, ct);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                service.IsAuthenticated = false;
                await service.SignInAsync(ct);
                response = await service.Client.ExecuteAsync<T>(request, method, ct);
            }
            return response.Data;
        }
    }
}
