using generate_idp_token.Pocos;

public interface ITokenService
{
    Task<AccessToken> GenerateToken();
}