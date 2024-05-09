using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System;
using viki_01.Entities;
using viki_01.Contexts;

namespace viki_01.Services
{
    public class SqlDbAuthService:IAuthService
    {
        private WikiHostingSqlServerContext appDbContext;

        public SqlDbAuthService(WikiHostingSqlServerContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public User Register(RegistrationCredentials credentials)
        {
            var isDuplicateEmail = appDbContext.Users.Any(u => String.Equals(u.Email, credentials.Email));
            var isDuplicateUsername = appDbContext.Users.Any(u => String.Equals(u.UserName, credentials.Username));

            if (isDuplicateEmail || isDuplicateUsername)
                throw new ArgumentException("User registration failed! Username or email is already taken!");

            var user = new User(credentials);
            appDbContext.Users.Add(user);
            appDbContext.SaveChanges();
            return user;
        }

        public AuthResponse Authenticate(SignInCredentials credentials)
        {
            var user = appDbContext.Users
                .FirstOrDefault(u => string.Equals(u.UserName, credentials.Username) && string.Equals(u.Password, credentials.Password));

            if (user == null)
                return null;

            var tokens = IssueTokens(user);
            var authResponse = new AuthResponse(tokens);
            return authResponse;
        }

        public AuthResponse RenewToken(string refreshToken)
        {
            if (String.IsNullOrEmpty(refreshToken))
                return null;

            var refreshTokenDbEntry = appDbContext.RefreshTokens.Find(refreshToken);
            if (refreshTokenDbEntry == null)
                return null;

            var user = appDbContext.Users.Find(refreshTokenDbEntry.UserId);
            var tokens = IssueTokens(user);
            var authResponse = new AuthResponse(tokens);
            appDbContext.RefreshTokens.Remove(refreshTokenDbEntry);
            appDbContext.SaveChanges();

            return authResponse;
        }

        public void DeleteExpiredTokens()
        {
            var cutoutTime = DateTime.UtcNow.Subtract(TimeSpan.FromHours(AuthOptions.REFRESH_TOKEN_LIFETIME_HOURS));
            var expiredTokens = appDbContext.RefreshTokens
                .Where(t => t.TimeCreated < cutoutTime)
                .ToList();
            appDbContext.RefreshTokens.RemoveRange(expiredTokens);
            appDbContext.SaveChanges();
        }

        public void Logout(string refreshToken)
        {
            if (String.IsNullOrEmpty(refreshToken))
                return;

            var refreshTokenDbEntry = appDbContext.RefreshTokens.Find(refreshToken);
            if (refreshTokenDbEntry == null)
                return;

            appDbContext.RefreshTokens.Remove(refreshTokenDbEntry);
            appDbContext.SaveChanges();
        }

        private TokenPair IssueTokens(User user)
        {
            if (user == null)
                return null;

            var claims = new List<Claim>();
            claims.Add(new Claim(AuthOptions.ClaimTypes.Email, user.Email));
            claims.Add(new Claim(AuthOptions.ClaimTypes.Id, user.Id.ToString()));
            claims.Add(new Claim(AuthOptions.ClaimTypes.Username, user.UserName));

            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
            notBefore: now,
            claims: claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.JWT_LIFETIME_MINUTES)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            var refreshToken = Guid.NewGuid().ToString();

            var tokenPair = new TokenPair
            {
                AccessToken = encodedJwt,
                RefreshToken = refreshToken
            };

            // save refresh token to DB
            var refreshTokenDbEntry = new RefreshToken
            {
                Token = refreshToken,
                TimeCreated = DateTime.UtcNow,
                User = user
            };
            appDbContext.RefreshTokens.Add(refreshTokenDbEntry);
            appDbContext.SaveChanges();

            return tokenPair;
        }

    }
}
