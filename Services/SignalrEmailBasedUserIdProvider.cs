using Microsoft.AspNetCore.SignalR;

namespace viki_01.Services
{
    public class SignalrEmailBasedUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(AuthOptions.ClaimTypes.Email)?.Value;
        }
    }
}