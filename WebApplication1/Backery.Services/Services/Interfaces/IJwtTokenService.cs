namespace WebApplication2.Backery.Services.Services.Interfaces;

public interface IJwtTokenService
{
    string CreateToken(string userId, IList<string> roles);

}
