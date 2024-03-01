using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using viki_01.Dto;
using viki_01.Entities;
using viki_01.Extensions;
using viki_01.Services;
using viki_01.Utils;

namespace viki_01.Controllers;

//TODO: add exceptions handling on controller actions, when will be generally determined how errors are handled and displayed for users
[ApiController]
[Route("/[controller]")]
public class TopicController(
    ITopicRepository topicRepository,
    ILoggerFactory loggerFactory) : ControllerBase
{
    private readonly ILogger<TopicController>
        logger = loggerFactory.CreateLogger<TopicController>();

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTopic([FromRoute] int id, [FromServices] IMapper<Topic, TopicDto> mapper)
    {
        logger.LogActionInformation(HttpMethods.Get, nameof(GetTopic), "Called with ID: {id}", id);

        var topic = await topicRepository.GetAsync(id);
        if (topic is null)
        {
            logger.LogActionWarning(HttpMethods.Get,
                nameof(GetTopic),
                "Topic with ID {id} not found",
                id);
            return NotFound();
        }

        logger.LogActionInformation(HttpMethods.Get,
            nameof(GetTopic),
            "Topic with ID {id} found and succesfully returned",
            id);
        return Ok(mapper.Map(topic));
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTopics([FromServices] IMapper<Topic, TopicDto> mapper, [FromQuery] string? search = null)
    {
        logger.LogActionInformation(HttpMethods.Get,
            nameof(GetTopics),
            "Called with search: {search}",
            search ?? "null");

        ICollection<Topic> topics;
        if (!string.IsNullOrWhiteSpace(search))
        {
            topics = await topicRepository.GetAllAsync(search);
        }
        else
        {
            topics = await topicRepository.GetAllAsync(search);
        }

        logger.LogActionInformation(HttpMethods.Get,
            nameof(GetTopics),
            "Succesfully returned {count} topics",
            topics.Count);
        return Ok(mapper.Map(topics));
    }

    [HttpPost]
    [Authorize("ModerationOnly")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateTopic([FromBody] TopicUpsertDto topicUpsertDto,
        [FromKeyedServices(nameof(ServiceKeys.LoggerSerializerOptions))]
        JsonSerializerOptions loggerSerializerOptions, [FromServices] IMapper<Topic, TopicUpsertDto> mapper)
    {
        var serializedRequestBody =
            JsonSerializer.Serialize(topicUpsertDto, loggerSerializerOptions);

        logger.LogActionInformation(HttpMethods.Post,
            nameof(CreateTopic),
            "Called with body: {requestBody}",
            serializedRequestBody);

        if (!ModelState.IsValid)
        {
            logger.LogActionWarning(HttpMethods.Post,
                nameof(CreateTopic),
                "Invalid request body: {requestBody}",
                serializedRequestBody);
            return BadRequest(ModelState);
        }

        var topic = mapper.Map(topicUpsertDto);
        await topicRepository.AddAsync(topic);

        logger.LogActionInformation(HttpMethods.Post,
            nameof(CreateTopic),
            "Successfully created topic with ID: {id}",
            topic.Id);
        return CreatedAtAction(nameof(GetTopic), new { id = topic.Id }, topic);
    }

    [HttpPut("{id:int}")]
    [Authorize("ModerationOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> EditTopic([FromRoute] int id,
        [FromBody] TopicUpsertDto topicUpsertDto,
        [FromKeyedServices(nameof(ServiceKeys.LoggerSerializerOptions))]
        JsonSerializerOptions loggerSerializerOptions,
        [FromServices] IMapper<Topic, TopicUpsertDto> mapper)
    {
        logger.LogActionInformation(HttpMethods.Put, nameof(EditTopic), "Called with ID: {id}", id);

        if (!ModelState.IsValid)
        {
            logger.LogActionWarning(HttpMethods.Put,
                nameof(EditTopic),
                "Invalid request body: {requestBody}",
                JsonSerializer.Serialize(topicUpsertDto, loggerSerializerOptions));

            return BadRequest(ModelState);
        }

        var topic = mapper.Map(topicUpsertDto);
        topic.Id = id;
        await topicRepository.EditAsync(id, topic);

        logger.LogActionInformation(HttpMethods.Put,
            nameof(EditTopic),
            "Succesfully edited topic with ID: {id}",
            id);
        
        return Ok(topic);
    }
    
    [HttpDelete("{id:int}")]
    [Authorize("ModerationOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteTopic([FromRoute] int id)
    {
        logger.LogActionInformation(HttpMethods.Delete, nameof(DeleteTopic), "Called with ID: {id}", id);
        
        await topicRepository.DeleteAsync(id);
        logger.LogActionInformation(HttpMethods.Delete, nameof(DeleteTopic), "Succesfully deleted topic with ID: {id}", id);
        
        return NoContent();
    }
}