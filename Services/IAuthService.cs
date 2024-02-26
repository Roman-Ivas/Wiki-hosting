using Microsoft.IdentityModel.Tokens;
using viki_01.Entities;

namespace viki_01.Services
{
    public interface IAuthService
    {
        AuthResponse Authenticate(SignInCredentials credentials);
        void DeleteExpiredTokens();
        void Logout(string refreshToken);
        User Register(RegistrationCredentials credentials);
        AuthResponse RenewToken(string refreshToken);
    }
}
