using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using viki_01.Contexts;
using viki_01.Entities;
using viki_01.Extensions;
using viki_01.Models.Dto;
using viki_01.Services;

namespace viki_01.Controllers;

[ApiController]
[Route("/[controller]")]
public class WikiController(IWikiRepository wikiRepository, ILoggerFactory loggerFactory) : ControllerBase
{
    private readonly ILogger<WikiController> logger = loggerFactory.CreateLogger<WikiController>();
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWikis([FromServices] IMapper<Wiki, WikiDto> mapper, [FromQuery] string? search = null, [FromQuery] string? topic = null)
    {
        logger.LogActionInformation(HttpMethods.Get, nameof(GetWikis), "Called with search: {search} and topic: {topic}", search ?? "null", topic ?? "null");
        var wikis = await wikiRepository.GetAllAsync(search, topic);
        
        logger.LogActionInformation(HttpMethods.Get, nameof(GetWikis), "Wikis found and succesfully returned");
        return Ok(mapper.Map(wikis));
    }
    
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWiki([FromRoute] int id, [FromServices] IMapper<Wiki, WikiDto> mapper, [FromServices] IHubContext<NotificationHub> notificationHub)
    {
        logger.LogActionInformation(HttpMethods.Get, nameof(GetWiki), "Called with ID: {id}", id);
        var wiki = await wikiRepository.GetAsync(id);
        if (wiki is null)
        {
            logger.LogActionWarning(HttpMethods.Get, nameof(GetWiki), "Wiki with ID {id} not found", id);
            return NotFound();
        }
        
        var contributors = wiki.Contributors.Select(c => new ContributorDto { Id = c.Id, UserId = c.UserId, UserName = c.User.UserName!, WikiId = c.WikiId, ContributorRoleId = c.ContributorRoleId });
        var mappedWiki = mapper.Map(wiki);
        mappedWiki.Contributors = contributors;
        
        logger.LogActionInformation(HttpMethods.Get, nameof(GetWiki), "Wiki with ID {id} found and succesfully returned", id);
        return Ok(mappedWiki);
    }
    
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateWiki([FromBody] WikiUpsertDto wikiDto, [FromServices] WikiHostingSqlServerContext context)
    {
        logger.LogActionInformation(HttpMethods.Post, nameof(CreateWiki), "Called with Name: {name}", wikiDto.Name);

        var topic = await context.Topics.Where(t => t.Name.ToUpper().Equals(wikiDto.Topic.ToUpper())).FirstOrDefaultAsync();
        if (topic is null)
        {
            logger.LogActionWarning(HttpMethods.Post, nameof(CreateWiki), "Topic with name {topic} not found", wikiDto.Topic);
            return BadRequest();
        }
        
        var wiki = new Wiki
        {
            Name = wikiDto.Name,
            BackgroundImagePath = wikiDto.BackgroundImagePath,
            MainWikiImagePath = wikiDto.MainWikiImagePath,
            Topics = new List<Topic> {topic},
            MainLinks = new List<Link>()
        };
    
        var createdWiki = await context.Wikis.AddAsync(wiki);
        await context.SaveChangesAsync();
        
        var ownerContributorRole =
            await context.ContributorRoles.Where(role => role.Name.ToUpper().Equals("OWNER")).FirstOrDefaultAsync();

        if (ownerContributorRole is null)
            return BadRequest("Server error. Wiki owner role is not defined");

        var contributor = new Contributor
        {
            UserId = HttpContext.User.GetId(),
            ContributorRoleId = ownerContributorRole.Id,
            WikiId = createdWiki.Entity.Id
        };
        
        await context.Contributors.AddAsync(contributor);
        await context.SaveChangesAsync();

        var mainPageHtml = $"<div><h1 style='font-weight: bold; color: white;'>Welcome to the {wiki.Name} wiki!</h1><div style='color: white'>We're a collaborative community website about {wiki.Name} that anyone, including you, can build and expand. Wikis like this one depend on readers getting involved and adding content. Click the \"ADD NEW PAGE\" or \"EDIT\" button at the top of any page to get started!</div></div>";
        var mainPage = new Page
        {
            AuthorId = HttpContext.User.GetId(),
            WikiId = createdWiki.Entity.Id,
            RawHtml = mainPageHtml,
            ProcessedHtml = mainPageHtml,
            CreatedAt = DateTime.Now,
            EditedAt = DateTime.Now
        };
        
        var createdPage = await context.Pages.AddAsync(mainPage);
        createdWiki.Entity.Pages.Add(createdPage.Entity);

        await context.SaveChangesAsync();
        logger.LogActionInformation(HttpMethods.Post, nameof(CreateWiki), "Successfully created wiki with Name: {name}", wikiDto.Name);
        return CreatedAtAction(nameof(GetWiki), new { id = wiki.Id }, wiki);
    }

    
    [HttpPut("{id:int}")]
    [Authorize("WikiOwner")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateWiki([FromRoute] int id, [FromBody] WikiUpsertDto wikiDto, [FromServices] WikiHostingSqlServerContext context)
    {
        logger.LogActionInformation(HttpMethods.Put, nameof(UpdateWiki), "Called with ID: {id}", id);
        var wiki = await context.Wikis.Where(w => w.Id == id).Include(w => w.MainLinks).Include(w => w.Contributors).FirstOrDefaultAsync();
        if (wiki is null)
        {
            logger.LogActionWarning(HttpMethods.Put, nameof(UpdateWiki), "Wiki with ID {id} not found", id);
            return NotFound();
        }

        if (!string.IsNullOrWhiteSpace(wikiDto.Name))
            wiki.Name = wikiDto.Name;

        if (!string.IsNullOrWhiteSpace(wikiDto.BackgroundImagePath))
            wiki.BackgroundImagePath = wikiDto.BackgroundImagePath;

        if (!string.IsNullOrWhiteSpace(wikiDto.MainWikiImagePath))
            wiki.MainWikiImagePath = wikiDto.MainWikiImagePath;
        
        if (!string.IsNullOrWhiteSpace(wikiDto.Topic))
        {
            var topic = await context.Topics.Where(t => t.Name.ToUpper().Equals(wikiDto.Topic.ToUpper())).FirstOrDefaultAsync();
            if (topic is not null)
                wiki.Topics = new List<Topic> {topic};
        }
        
        // Find links to remove
        var linksToRemove = wiki.MainLinks.Where(link => wikiDto.MainLinks.All(dtoLink => dtoLink.Id != link.Id)).ToList();

        // Remove the links
        foreach (var link in linksToRemove)
        {
            var linkEntry = await context.Links.FindAsync(link.Id);
            if (linkEntry is not null)
            {
                wiki.MainLinks.Remove(linkEntry);
                context.Links.Remove(linkEntry);
            }
        }

        // Find links to add
        var linksToAdd = wikiDto.MainLinks.Where(dtoLink => wiki.MainLinks.All(link => link.Id != dtoLink.Id)).ToList();
 
        // Add the new links
        foreach (var link in linksToAdd)
        {
            var linkEntry = await context.Links.FindAsync(link.Id);
            if (linkEntry is null)
            {
                if (string.IsNullOrWhiteSpace(link.Title) || string.IsNullOrWhiteSpace(link.Url))
                    continue;
                
                linkEntry = new Link { Title = link.Title, Url = link.Url, WikiId = id }; 
                await context.Links.AddAsync(linkEntry);
            }
            
            wiki.MainLinks.Add(linkEntry);
        }
        
        // Find contributors to remove
            var contributorsToRemove = wiki.Contributors.Where(contributor => wikiDto.Contributors.All(dtoContributor => dtoContributor.UserId != contributor.UserId)).ToList();
        
            // Remove the contributors
            foreach (var contributor in contributorsToRemove)
            {
                var contributorEntry = await context.Contributors.FindAsync(contributor.Id);
                if (contributorEntry is not null)
                {
                    wiki.Contributors.Remove(contributorEntry);
                    context.Contributors.Remove(contributorEntry);
                }
            }
            
            var contributorsToUpdate = wiki.Contributors.Where(contributor => wikiDto.Contributors.Any(dtoContributor => dtoContributor.UserId == contributor.UserId && dtoContributor.ContributorRoleId != contributor.ContributorRoleId)).ToList();

            // Update the contributors
            foreach (var contributor in contributorsToUpdate)
            {
                var dtoContributor = wikiDto.Contributors.First(c => c.UserId == contributor.UserId);
                contributor.ContributorRoleId = dtoContributor.ContributorRoleId;
            }
        
            // Find contributors to add
            var contributorsToAdd = wikiDto.Contributors.Where(dtoContributor => wiki.Contributors.All(contributor => contributor.UserId != dtoContributor.UserId)).ToList();
        
            // Add the new contributors
            foreach (var contributor in contributorsToAdd)
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.UserName!.Equals(contributor.UserName));
                if (user is null)
                {
                    // Handle the case where the user is not found
                    continue;
                }
        
                var contributorEntry = new Contributor { UserId = user.Id, WikiId = id, ContributorRoleId = contributor.ContributorRoleId };
                await context.Contributors.AddAsync(contributorEntry);
                wiki.Contributors.Add(contributorEntry);
            }

        await context.SaveChangesAsync();
        await wikiRepository.EditAsync(id, wiki);
        logger.LogActionInformation(HttpMethods.Put, nameof(UpdateWiki), "Successfully updated wiki with ID: {id}", id);
        return NoContent();
    }
    
    [HttpPost("/archive/{id:int}")]
    [Authorize("WikiOwner")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ArchiveWiki([FromRoute] int id)
    {
        logger.LogActionInformation(HttpMethods.Post, nameof(ArchiveWiki), "Called with ID: {id}", id);
        var wiki = await wikiRepository.GetAsync(id);
        if (wiki is null)
        {
            logger.LogActionWarning(HttpMethods.Post, nameof(ArchiveWiki), "Wiki with ID {id} not found", id);
            return NotFound();
        }
        
        wiki.IsArchived = true;
        await wikiRepository.EditAsync(id, wiki);
        logger.LogActionInformation(HttpMethods.Post, nameof(ArchiveWiki), "Succesfully archived wiki with ID: {id}", id);
        return NoContent();
    }

    [HttpPost("/merge/{firstWikiId:int}/{secondWikiId:int}")]
    [Authorize("WikiOwner")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> MergeWikis([FromRoute] int firstWikiId,
        [FromRoute] int secondWikiId, [FromServices] IPageRepository pageRepository)
    {
        logger.LogActionInformation(HttpMethods.Post,
            nameof(MergeWikis),
            "Called with firstWikiId: {firstWikiId} and secondWikiId: {secondWikiId}",
            firstWikiId,
            secondWikiId);
        var firstWiki = await wikiRepository.GetAsync(firstWikiId);
        if (firstWiki is null)
        {
            logger.LogActionWarning(HttpMethods.Post,
                nameof(MergeWikis),
                "Wiki with ID {firstWikiId} not found",
                firstWikiId);
            return NotFound();
        }

        var secondWiki = await wikiRepository.GetAsync(secondWikiId);
        if (secondWiki is null)
        {
            logger.LogActionWarning(HttpMethods.Post,
                nameof(MergeWikis),
                "Wiki with ID {secondWikiId} not found",
                secondWikiId);
            return NotFound();
        }
        
        var secondWikiPages = await pageRepository.GetAllAsync(secondWikiId);
        foreach (var page in secondWikiPages)
        {
            page.WikiId = firstWikiId;
            await pageRepository.EditAsync(page.Id, page);
        }
        
        await wikiRepository.DeleteAsync(secondWikiId);
        logger.LogActionInformation(HttpMethods.Post,
            nameof(MergeWikis),
            "Succesfully merged wikis with ID {firstWikiId} and {secondWikiId}",
            firstWikiId,
            secondWikiId);
        
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize("WikiOwner")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteWiki([FromRoute] int id)
    {
        logger.LogActionInformation(HttpMethods.Delete, nameof(DeleteWiki), "Called with ID: {id}", id);
        
        //TODO: Before deleting wiki, add iterating over each wiki page and delete all related media contents
        await wikiRepository.DeleteAsync(id);
        logger.LogActionInformation(HttpMethods.Delete, nameof(DeleteWiki), "Succesfully deleted wiki with ID: {id}", id);
        return NoContent();
    }
    
    [HttpGet("contributors/roles")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetContributorRoles([FromServices] WikiHostingSqlServerContext context)
    {
        logger.LogActionInformation(HttpMethods.Get, nameof(GetContributorRoles), "Called");
        return Ok(await context.ContributorRoles.ToListAsync());
    }
    
    [HttpGet("{id:int}/contributors")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWikiContributors([FromRoute] int id, [FromQuery] string? role, [FromServices] IContributorRepository contributorRepository)
    {
        logger.LogActionInformation(HttpMethods.Get, nameof(GetWikiContributors), "Called with ID: {id}", id);
        var wiki = await wikiRepository.GetAsync(id);
        if (wiki is null)
        {
            logger.LogActionWarning(HttpMethods.Get, nameof(GetWikiContributors), "Wiki with ID {id} not found", id);
            return NotFound();
        }
        
        logger.LogActionInformation(HttpMethods.Get, nameof(GetWikiContributors), "Wiki with ID {id} found and succesfully returned", id);
        return Ok(await contributorRepository.GetWikiContributors(id, role));
    }
    
    [HttpPost("{id:int}/contributors/{userId}")]
    [Authorize("WikiOwner")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddWikiContributor([FromRoute] int id, [FromRoute] int userId, [FromBody] ContributorUpsertDto contributorUpsertDto, [FromServices] IContributorRepository contributorRepository)
    {
        logger.LogActionInformation(HttpMethods.Post, nameof(AddWikiContributor), "Called with ID: {id}", id);
        var wiki = await wikiRepository.GetAsync(id);
        if (wiki is null)
        {
            logger.LogActionWarning(HttpMethods.Post, nameof(AddWikiContributor), "Wiki with ID {id} not found", id);
            return NotFound();
        }
        
        await contributorRepository.AddAsync(new Contributor
        {
            UserId = userId,
            WikiId = id,
            ContributorRoleId = contributorUpsertDto.ContributorRoleId
        });
        
        logger.LogActionInformation(HttpMethods.Post, nameof(AddWikiContributor), "Succesfully added contributor with ID: {userId} to wiki with ID: {id}", userId, id);
        return NoContent();
    }
    
    [HttpDelete("{id:int}/contributors/{userId}")]
    [Authorize("WikiOwner")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RemoveWikiContributor([FromRoute] int id, [FromRoute] int userId, [FromServices] IContributorRepository contributorRepository)
    {
        logger.LogActionInformation(HttpMethods.Delete, nameof(RemoveWikiContributor), "Called with ID: {id}", id);
        var wiki = await wikiRepository.GetAsync(id);
        if (wiki is null)
        {
            logger.LogActionWarning(HttpMethods.Delete, nameof(RemoveWikiContributor), "Wiki with ID {id} not found", id);
            return NotFound();
        }

        var contributor = await contributorRepository.GetContributor(id, userId);
        if (contributor is null)
        {
            logger.LogActionWarning(HttpMethods.Delete, nameof(RemoveWikiContributor), "Contributor with ID {userId} not found in wiki with ID: {id}", userId, id);
            return NotFound();
        }
        
        await contributorRepository.DeleteAsync(contributor.Id);
        logger.LogActionInformation(HttpMethods.Delete, nameof(RemoveWikiContributor), "Succesfully removed contributor with ID: {userId} from wiki with ID: {id}", userId, id);
        return NoContent();
    }
    
    
    
}