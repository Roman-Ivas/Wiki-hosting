using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using viki_01.Authorization;
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