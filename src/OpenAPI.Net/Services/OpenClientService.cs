using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using OpenAPI.Net.Auth;
using OpenAPI.Net.Helpers;
using OpenAPI.Net.Interfaces;

namespace OpenAPI.Net.Services;


public class OpenClientService(
    OpenClient client,
    ConnectionInfo logonInfo,
    ILogger logger,
    int timeoutSecond) : IOpenClientService
{
    public void Dispose() => DisposeAsync().AsTask().GetAwaiter().GetResult();

    public async ValueTask DisposeAsync()
    {
        await LogoutAsync();

        if (client?.IsTerminated == false)
            client.Dispose();

    }

    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        var logoutReq = new ProtoOAAccountLogoutReq
        {
            PayloadType = ProtoOAPayloadType.ProtoOaAccountLogoutReq
        };
        await client.SendMessage(logoutReq);
    }

    public async Task<Token?> RefreshTokenAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await client.Connect();
            var appAuthReq = new ProtoOAApplicationAuthReq()
            {
                ClientId = logonInfo.ClientId,
                ClientSecret = logonInfo.ClientSecret
            };
            var appAuthRes = await SendAsync<ProtoOAApplicationAuthReq, ProtoOAApplicationAuthRes>(appAuthReq, cancellationToken);

            var refreshReq = new ProtoOARefreshTokenReq
            {
                RefreshToken = logonInfo.Token.RefreshToken
            };
            var refreshRes = await SendAsync<ProtoOARefreshTokenReq, ProtoOARefreshTokenRes>(refreshReq, cancellationToken);

            if (!refreshRes.HasAccessToken) return null;

            return new Token
            {
                AccessToken = refreshRes.AccessToken,
                RefreshToken = refreshRes.RefreshToken,
                ExpiresIn = DateTimeOffset.UtcNow.AddSeconds(refreshRes.ExpiresIn),
                TokenType = refreshRes.TokenType
            };

        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<Exception?> LogonAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await client.Connect();
            var appAuthReq = new ProtoOAApplicationAuthReq()
            {
                ClientId = logonInfo.ClientId,
                ClientSecret = logonInfo.ClientSecret
            };

            var accAuthReq = new ProtoOAAccountAuthReq
            {
                CtidTraderAccountId = logonInfo.AccountId,
                AccessToken = logonInfo.Token.AccessToken
            };


            var appAuthRes = await SendAsync<ProtoOAApplicationAuthReq, ProtoOAApplicationAuthRes>(appAuthReq, cancellationToken);
            var accAuthRes = await SendAsync<ProtoOAAccountAuthReq, ProtoOAAccountAuthRes>(accAuthReq, cancellationToken);

            return null;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IMessage<TRequest>
        where TResponse : IMessage<TResponse>
    {
        if (client is null || client?.IsTerminated == true)
            throw new InvalidOperationException("OpenClient is not connected.");

        var responseReceiving = new TaskCompletionSource<TResponse>();
        try
        {
            var payloadType = Enum.Parse<ProtoOAPayloadType>(request.Descriptor.FullName, ignoreCase: true);

            request.SetField(nameof(ProtoOAAccountAuthReq.CtidTraderAccountId), logonInfo.AccountId);


            using var subscription = SubscribeResponse(responseReceiving);

            await client.SendMessage(request, payloadType);

            var response = await responseReceiving
                .Task
                .WaitAsync(TimeSpan.FromSeconds(timeoutSecond), cancellationToken);

            return response;
        }
        catch (TimeoutException timeoutException)
        {
            logger.LogWarning(timeoutException, $"Request {typeof(TRequest).Name} timed out after {timeoutSecond} seconds.");
            responseReceiving.TrySetCanceled();
            throw;
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation($"Request {typeof(TRequest).Name} was cancelled.");
            responseReceiving.TrySetCanceled();
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            responseReceiving.TrySetCanceled();
            throw;
        }
    }

    private IDisposable SubscribeResponse<TResponse>(TaskCompletionSource<TResponse> responseReceived)
        where TResponse : IMessage<TResponse>
    {
        return client.Where(x => x is not ProtoHeartbeatEvent).Subscribe(msg =>
        {
            if (msg is TResponse res)
                responseReceived.TrySetResult(res);
        },
        error =>
        {
            logger.LogError(error, "Error occurred while processing response.");
            responseReceived.TrySetException(error);
        },
        () =>
        {
            logger.LogInformation("Response stream completed.");
            responseReceived.TrySetCanceled();
        });
    }
}

