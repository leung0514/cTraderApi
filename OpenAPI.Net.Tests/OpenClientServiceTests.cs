using FakeItEasy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAPI.Net.Auth;
using OpenAPI.Net.Helpers;
using OpenAPI.Net.Services;
using Shouldly;

namespace OpenAPI.Net.Tests;

public class OpenClientServiceTests
{
    [Fact]
    public async Task ShouldRefreshToken()
    {
        //Arrange
        var service = GetOpenClientService();

        //Act
        var newToken = await service.RefreshTokenAsync();

        //Arrange
        newToken?.AccessToken.ShouldNotBeNullOrEmpty();
    }

    private static OpenClientService GetOpenClientService()
    {
        var config = GetConfig();
        var connInfo = config.GetRequiredSection(nameof(ConnectionInfo)).Get<ConnectionInfo>() ??
            throw new InvalidOperationException("LogonInfo configuration is missing or invalid.");
        var host = ApiInfo.GetHost(connInfo.Mode);
        var client = new OpenClient(host, ApiInfo.Port, TimeSpan.FromSeconds(connInfo.TimeoutSecond));
        var logger = A.Fake<ILogger<OpenClientService>>();
        var service = new OpenClientService(client, connInfo, logger, connInfo.TimeoutSecond);
        return service;
    }

    private static IConfigurationRoot GetConfig()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("config.json", optional: false, reloadOnChange: true)
            .Build();
    }
}



