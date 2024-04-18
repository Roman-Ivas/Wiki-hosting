using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using viki_01.Contexts;
using viki_01.Extensions;

namespace viki_01.Authorization;

public class TemplateOwnerAuthorizationHandler(WikiHostingSqlServerContext databaseContext)
    : IAuthorizationHandler
{
    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var pendingRequirements = context.PendingRequirements.ToList();

        foreach (var requirement in pendingRequirements)
        {
            if (requirement is not OperationAuthorizationRequirement { Name: "TemplateOwner" }) continue;

            var userId = context.User.GetId();

            if (context.Resource is not HttpContext httpContext) continue;
            if (!httpContext.Request.RouteValues.TryGetValue("id", out var templateIdObject)) continue;
            var templateId = Convert.ToInt32(templateIdObject);
            if (templateId == 0) continue;

            var template = await databaseContext.Templates.FindAsync(templateId);
            if (template is null) continue;

            if (template.AuthorId == userId || context.User.IsInRole("Moderator") || context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
            }
        }
    }
}
