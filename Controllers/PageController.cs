using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using viki_01.Entities;
using viki_01.Extensions;
using viki_01.Models.Dto;
using viki_01.Services;
using viki_01.Utils;

namespace viki_01.Controllers;

//TODO: add exceptions handling on controller actions, when will be generally determined how errors are handled and displayed for users
[ApiController]
[Route("/[controller]")]
public class PageController(
    IPageRepository pageRepository,
    ILoggerFactory loggerFactory) : ControllerBase
{
    private readonly ILogger<PageController> logger = loggerFactory.CreateLogger<PageController>();

    [HttpGet("wiki/{wikiId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPagesByWikiId([FromRoute] int wikiId,
    [FromServices] IMapper<Page, PageDto> mapper)
    {
        logger.LogActionInformation(HttpMethods.Get, nameof(GetPagesByWikiId), "Called with Wiki ID: {wikiId}", wikiId);

        var pages = await pageRepository.GetAllAsync(wikiId);
        if (pages == null || pages.Count == 0)
        {
            logger.LogActionWarning(HttpMethods.Get,
                nameof(GetPagesByWikiId),
                "No pages found for Wiki ID {wikiId}",
                wikiId);
            return NotFound();
        }

        logger.LogActionInformation(HttpMethods.Get,
            nameof(GetPagesByWikiId),
            "Pages found for Wiki ID {wikiId} and successfully returned",
            wikiId);
        return Ok(pages.Select(page => mapper.Map(page)));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPage([FromRoute] int id,
        [FromServices] IMapper<Page, PageDto> mapper)
    {
        logger.LogActionInformation(HttpMethods.Get, nameof(GetPage), "Called with ID: {id}", id);

        var page = await pageRepository.GetAsync(id);
        if (page is null)
        {
            logger.LogActionWarning(HttpMethods.Get,
                nameof(GetPage),
                "Page with ID {id} not found",
                id);
            return NotFound();
        }

        logger.LogActionInformation(HttpMethods.Get,
            nameof(GetPage),
            "Page with ID {id} found and succesfully returned",
            id);
        return Ok(mapper.Map(page));
    }

    [HttpGet("{wikiTitle}/{pageTitle}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPage([FromRoute] string wikiTitle,
        [FromRoute] string pageTitle,
        [FromServices] IMapper<Page, PageDto> mapper)
    {
        logger.LogActionInformation(HttpMethods.Get, nameof(GetPage), "Called with wiki name: {wikiName} and page name: {pageName}", wikiTitle, pageTitle);

        var decodedWikiTitle = System.Net.WebUtility.UrlDecode(wikiTitle);
        var decodedPageTitle = System.Net.WebUtility.UrlDecode(pageTitle);

        var page = await pageRepository.GetAsync(decodedWikiTitle, decodedPageTitle);
        if (page is null)
        {
            logger.LogActionWarning(HttpMethods.Get,
                nameof(GetPage),
                "Page with wiki name {wikiName} and page name {pageName} not found",
                wikiTitle,
                pageTitle);
            return NotFound();
        }

        logger.LogActionInformation(HttpMethods.Get,
            nameof(GetPage),
            "Page with wiki name {wikiName} and page name {pageName} found and succesfully returned",
            wikiTitle,
            pageTitle);
        return Ok(mapper.Map(page));
    }
    
    [HttpGet($"{nameof(GetMainWikiPage)}/{{wikiId:int}}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMainWikiPage([FromRoute] int wikiId,
        [FromServices] IMapper<Page, PageDto> mapper)
    {
        logger.LogActionInformation(HttpMethods.Get, nameof(GetMainWikiPage), "Called with wiki ID: {wikiId}", wikiId);

        var pages = await pageRepository.GetAllAsync(wikiId);
        if (pages.Count < 1)
        {
            logger.LogActionWarning(HttpMethods.Get,
                nameof(GetMainWikiPage),
                "Pages for wiki with ID {wikiId} not found",
                wikiId);
            
            return NotFound();
        }

        var page = pages.MinBy(page => page.CreatedAt);
        if (page is null)
        {
            logger.LogActionWarning(HttpMethods.Get,
                nameof(GetMainWikiPage),
                "Main page for wiki with ID {wikiId} not found",
                wikiId);
            return NotFound();
        }

        logger.LogActionInformation(HttpMethods.Get,
            nameof(GetMainWikiPage),
            "Main page for wiki with ID {wikiId} found and succesfully returned",
            wikiId);
        
        return Ok(mapper.Map(page));
    }

    [HttpGet(nameof(GetRelevantWikiPages))]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetRelevantWikiPages([FromQuery] int limit = 20)
    {
        logger.LogActionInformation(HttpMethods.Get, nameof(GetRelevantWikiPages), "Called with limit: {limit}", limit);

        var pages = await pageRepository.GetRelevantPagesAsync(limit);
        logger.LogActionInformation(HttpMethods.Get, nameof(GetRelevantWikiPages), "Succesfully returned relevant pages");
        
        return Ok(pages);
    }

    [HttpPost("{wikiId:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreatePage([FromRoute] int wikiId,
        [FromBody] PageUpsertDto pageUpsertDto,
        [FromKeyedServices(nameof(ServiceKeys.LoggerSerializerOptions))]
        JsonSerializerOptions loggerSerializerOptions,
        [FromServices] IMapper<Page, PageUpsertDto> mapper)
    {
        logger.LogActionInformation(HttpMethods.Post,
            nameof(CreatePage),
            "Called with wiki ID: {wikiId} and request body: {requestBody}",
            wikiId,
            JsonSerializer.Serialize(pageUpsertDto, loggerSerializerOptions));

        var page = mapper.Map(pageUpsertDto);
        page.WikiId = wikiId;
        page.AuthorId = HttpContext.User.GetId();
        page.RawHtml = page.RawHtml ?? string.Empty;
        page.ProcessedHtml = page.ProcessedHtml ?? string.Empty;

        await pageRepository.AddAsync(page);
        logger.LogActionInformation(HttpMethods.Post,
            nameof(CreatePage),
            "Succesfully created page with ID: {id}",
            page.Id);

        return CreatedAtAction(nameof(GetPage), new { id = page.Id }, page);
    }

    [HttpPut("{id:int}")]
    [Authorize("PageUpsert")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> EditPage([FromRoute] int id,
        [FromBody] PageUpsertDto pageUpsertDto,
        [FromKeyedServices(nameof(ServiceKeys.LoggerSerializerOptions))]
        JsonSerializerOptions loggerSerializerOptions,
        [FromServices] IMapper<Page, PageUpsertDto> mapper)
    {
        logger.LogActionInformation(HttpMethods.Put, nameof(EditPage), "Called with ID: {id} and body: {requestBody}", id, JsonSerializer.Serialize(pageUpsertDto, loggerSerializerOptions));
        
        var page = await pageRepository.GetAsync(id);
        if (page is null)
        {
            logger.LogActionWarning(HttpMethods.Put, nameof(EditPage), "Page with ID {id} not found", id);
            return BadRequest();
        }

        mapper.Map(mapper.Map(pageUpsertDto), page);
        await pageRepository.EditAsync(id, page);
        logger.LogActionInformation(HttpMethods.Put, nameof(EditPage), "Succesfully edited page with ID: {id}", id);
        
        return Ok(page);
    }
    
    [HttpDelete("{id:int}")]
    [Authorize("PageUpsert")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePage([FromRoute] int id)
    {
        logger.LogActionInformation(HttpMethods.Delete, nameof(DeletePage), "Called with ID: {id}", id);
        
        var page = await pageRepository.GetAsync(id);
        if (page is null)
        {
            logger.LogActionWarning(HttpMethods.Delete, nameof(DeletePage), "Page with ID {id} not found", id);
            return NotFound();
        }
        
       //TODO: add deleting all related mediacontent before deleting page 
        
        await pageRepository.DeleteAsync(id);
        logger.LogActionInformation(HttpMethods.Delete, nameof(DeletePage), "Succesfully deleted page with ID: {id}", id);
        
        return NoContent();
    }
}