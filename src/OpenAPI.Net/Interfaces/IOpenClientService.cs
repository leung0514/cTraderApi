using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;

namespace OpenAPI.Net.Interfaces;

public interface IOpenClientService : IDisposable, IAsyncDisposable
{
    Task<Exception?> LogonAsync(CancellationToken cancellationToken = default);
    Task LogoutAsync(CancellationToken cancellationToken = default);
    Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IMessage<TRequest>
        where TResponse : IMessage<TResponse>;
}
