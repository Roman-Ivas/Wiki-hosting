using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using viki_01.Extensions;
using viki_01.Services;

namespace viki_01.Authorization;

public class PageAuthorizationHandler(IContributorRepository contributorRepository, IPageRepository pageRepository) : IAuthorizationHandler
{
    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var pendingRequrements = context.PendingRequirements.ToList();

        foreach (var requirement in pendingRequrements)
        {
            if (requirement is not OperationAuthorizationRequirement { Name: "PageUpsert" }) continue;
            var userId = context.User.GetId();

            if (context.Resource is not HttpContext httpContext) continue;
            if (!httpContext.Request.RouteValues.TryGetValue("id", out var pageIdObject)) continue;
            var pageId = Convert.ToInt32(pageIdObject);
            if (pageId == 0) continue;
            
            var page = await pageRepository.GetAsync(pageId);
            if (page is null) continue;
            
            var contributor =
                await contributorRepository.GetContributor(page.WikiId, userId);

            if (contributor is not null || context.User.IsInRole("Moderator") || context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
            }
        }
    }
}