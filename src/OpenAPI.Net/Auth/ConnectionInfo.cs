using OpenAPI.Net.Helpers;

namespace OpenAPI.Net.Auth;

public sealed class ConnectionInfo
{
    public Mode Mode { get; set; }
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public long AccountId { get; set; }
    public Token Token { get; set; }

    public int TimeoutSecond { get; set; } = 10;
}