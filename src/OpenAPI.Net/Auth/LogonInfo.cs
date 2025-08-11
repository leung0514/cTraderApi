using OpenAPI.Net.Helpers;

namespace OpenAPI.Net.Auth;

public sealed class LogonInfo
{
    public Mode Mode { get; set; }
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public long AccountId { get; set; }
    public string AccessToken { get; set; } = string.Empty;
}