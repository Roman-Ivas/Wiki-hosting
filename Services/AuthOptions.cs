using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace viki_01.Services
{
    public class AuthOptions
    {
        public static class ClaimTypes
        {
            public const string Id = "user_id";
            public const string Email = "user_email";
            public const string Username = "username";
        }

        public const string ISSUER = "WebChatServer";
        public const string AUDIENCE = "WebChatClient";
        const string KEY = "f96f4463-f0fe-4539-badb-229d8fcb9ccd";
        public const int JWT_LIFETIME_MINUTES = 60;
        public const int REFRESH_TOKEN_LIFETIME_HOURS = 24;

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }

        public static void ConfigureJwtBearer(JwtBearerOptions options)
        {
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = AuthOptions.ISSUER,
                ValidateAudience = true,
                ValidAudience = AuthOptions.AUDIENCE,
                ValidateLifetime = true,
                IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                ValidateIssuerSigningKey = true,
            };
            //options.Events = new JwtBearerEvents
            //{
            //    OnMessageReceived = context => {
            //        var accessToken = context.Request.Query["access_token"];

            //        var path = context.HttpContext.Request.Path;
            //        if (!string.IsNullOrEmpty(accessToken) &&
            //            path.StartsWithSegments("/chat"))
            //        {
            //            context.Token = accessToken;
            //        }
            //        return Task.CompletedTask;
            //    }
            //};
        }
    }
}
