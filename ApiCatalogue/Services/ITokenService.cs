using ApiCatalogue.Models;

namespace ApiCatalogue.Services;
public interface ITokenService
{
    string GenerationToken(string key, string issuer, string audience, UserModel userModel);

}

