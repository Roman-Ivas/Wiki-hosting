using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using viki_01.Extensions;
using viki_01.Services;

namespace viki_01.Authorization;

public class WikiOwnerAuthorizationHandler(IContributorRepository contributorRepository, IWikiRepository wikiRepository) : IAuthorizationHandler
{
    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var pendingRequirements = context.PendingRequirements.ToList();

        foreach (var requirement in pendingRequirements)
        {
            if (requirement is not OperationAuthorizationRequirement
                {
                    Name: "WikiOwner"
                }) continue;
            
            var userId = context.User.GetId();
            
            if (context.Resource is not HttpContext httpContext) continue;
            if (!httpContext.Request.RouteValues.TryGetValue("id", out var wikiIdObject)) continue;
            var wikiId = Convert.ToInt32(wikiIdObject);
            if (wikiId == 0) continue;
            
            var wiki = await wikiRepository.GetAsync(wikiId);
            if (wiki is null) continue;
            
            var contributor = await contributorRepository.GetContributor(wikiId, userId);
            if (contributor is null) continue;

            var contributorRole = contributor.ContributorRole.Name.ToUpper();
            
            if (contributorRole == "OWNER" || context.User.IsInRole("Moderator") || context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
            }
        }
    }
}