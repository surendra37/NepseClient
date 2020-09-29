using System;

namespace NepseClient.Commons.Contracts
{

    public interface IAuthenticatableNepseClient : INepseClient
    {
        Action ShowAuthenticationDialog { get; set;}
    }
}