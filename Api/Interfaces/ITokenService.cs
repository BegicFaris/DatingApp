using Api.Entities;

namespace Api.Interfaceces;
public interface ITokenService
{
    string CreateToken(AppUser user);
}