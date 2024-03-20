using generate_idp_token.Pocos;

public class TokenService : ITokenService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IdpConfiguration _idpConfiguration;

    public TokenService(IHttpClientFactory httpClientFactory, IdpConfiguration idpConfiguration)
    {
        _httpClientFactory = httpClientFactory;
        _idpConfiguration = idpConfiguration;
    }
    public async Task<AccessToken> GenerateToken()
    {
        var client = _httpClientFactory.CreateClient("IDP");
        var content = new FormUrlEncodedContent(
                    [
                        new("client_id", _idpConfiguration.ClientId),
                        new("client_secret", _idpConfiguration.ClientSecret),
                        new("scope", _idpConfiguration.Scope),
                        new("grant_type", _idpConfiguration.GrantType)
                    ]);

        var response = await client.PostAsync("connect/token", content);
        if (response.IsSuccessStatusCode)
        {
            var token = await response.Content.ReadAsStringAsync();
            return System.Text.Json.JsonSerializer.Deserialize<AccessToken>(token, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        else
        {
            throw new Exception("Failed to generate token");
        }
    }
}