using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using viki_01.Dto;
using viki_01.Entities;
using viki_01.Extensions;
using viki_01.Services;

namespace viki_01.Controllers;

[ApiController]
[Route("/[controller]")]
public class WikiController(IWikiRepository wikiRepository, ILoggerFactory loggerFactory) : ControllerBase
{
    private readonly ILogger<WikiController> logger = loggerFactory.CreateLogger<WikiController>();
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWikis([FromServices] IMapper<Wiki, WikiDto> mapper, [FromQuery] string? search = null)
    {
        logger.LogActionInformation(HttpMethods.Get, nameof(GetWikis), "Called with search: {search}", search ?? "null");
        ICollection<Wiki> wikis;
        if (!string.IsNullOrWhiteSpace(search))
        {
            wikis = await wikiRepository.GetAllAsync(search);
        }
        else
        {
            wikis = await wikiRepository.GetAllAsync(search);
        }
        
        logger.LogActionInformation(HttpMethods.Get, nameof(GetWikis), "Wikis found and succesfully returned");
        return Ok(mapper.Map(wikis));
    }
    
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWiki([FromRoute] int id, [FromServices] IMapper<Wiki, WikiDto> mapper)
    {
        logger.LogActionInformation(HttpMethods.Get, nameof(GetWiki), "Called with ID: {id}", id);
        var wiki = await wikiRepository.GetAsync(id);
        if (wiki is null)
        {
            logger.LogActionWarning(HttpMethods.Get, nameof(GetWiki), "Wiki with ID {id} not found", id);
            return NotFound();
        }
        
        logger.LogActionInformation(HttpMethods.Get, nameof(GetWiki), "Wiki with ID {id} found and succesfully returned", id);
        return Ok(mapper.Map(wiki));
    }
    
    [HttpPut("{id:int}")]
    [Authorize("WikiOwner")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateWiki([FromRoute] int id, [FromBody] WikiUpsertDto wikiDto, [FromServices] IMapper<Wiki, WikiUpsertDto> mapper)
    {
        logger.LogActionInformation(HttpMethods.Put, nameof(UpdateWiki), "Called with ID: {id}", id);
        var wiki = await wikiRepository.GetAsync(id);
        if (wiki is null)
        {
            logger.LogActionWarning(HttpMethods.Put, nameof(UpdateWiki), "Wiki with ID {id} not found", id);
            return NotFound();
        }
        
        //TODO: Upgrade mapper to be able directly mapping from object to object
        mapper.Map(mapper.Map(wikiDto), wiki);
        await wikiRepository.EditAsync(id, wiki);
        logger.LogActionInformation(HttpMethods.Put, nameof(UpdateWiki), "Succesfully updated wiki with ID: {id}", id);
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
}