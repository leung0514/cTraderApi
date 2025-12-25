using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenAPI.Net.Auth;
using OpenAPI.Net.Helpers;
using OpenAPI.Net.Interfaces;
using OpenAPI.Net.Services;

namespace OpenAPI.Net;

public static class DependencyInjection
{
    public static IServiceCollection AddOpenClientService(
    this IServiceCollection services,
    ConnectionInfo logonInfo,
    int timeoutSecond = 10)
    {
        services.AddTransient(provider =>
        {
            var host = ApiInfo.GetHost(logonInfo.Mode);
            return new OpenClient(host, ApiInfo.Port, TimeSpan.FromSeconds(timeoutSecond));
        });
        services.AddTransient<IOpenClientService, OpenClientService>(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<OpenClientService>>();
            var client = provider.GetRequiredService<OpenClient>();
            return new OpenClientService(client, logonInfo, logger, timeoutSecond);
        });
        return services;
    }

}
