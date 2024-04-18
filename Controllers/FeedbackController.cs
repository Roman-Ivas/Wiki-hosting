using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using viki_01.Entities;
using viki_01.Extensions;
using viki_01.Models.Dto;
using viki_01.Services;

namespace viki_01.Controllers;

[ApiController]
[Route("[controller]")]
public class FeedbackController(IFeedbackRepository feedbackRepository, ILoggerFactory loggerFactory) : ControllerBase
{
    private readonly ILogger<FeedbackController> logger =
        loggerFactory.CreateLogger<FeedbackController>();
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFeedbacks()
    {
        var feedbacks = await feedbackRepository.GetFeedbacksAsync();
        return Ok(feedbacks);
    }
    
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFeedback(int id)
    {
        var feedback = await feedbackRepository.GetFeedbackAsync(id);
        if (feedback is null)
        {
            return NotFound();
        }
        return Ok(feedback);
    }
    
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddFeedback(FeedbackDto feedbackDto, [FromServices] IMapper<Feedback, FeedbackDto> mapper)
    {
        var feedback = mapper.Map(feedbackDto);
        var userId = HttpContext.User.GetId();
        feedback.UserId = userId;
        feedback.PostedAt = DateTime.Now;
        
        await feedbackRepository.CreateFeedbackAsync(feedback);
        return CreatedAtAction(nameof(GetFeedback), new { id = feedback.Id }, feedback);
    }
}