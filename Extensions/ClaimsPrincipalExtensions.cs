using System.Security.Claims;

namespace viki_01.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetId(this ClaimsPrincipal user)
    {
        var nameIdentifier = user.Claims.FirstOrDefault(claim => claim.Type.Equals("user_id"))?.Value;
        if (nameIdentifier is null || !int.TryParse(nameIdentifier, out var id))
            throw new InvalidOperationException("User is not authorized");
        
        return id;
    }
}