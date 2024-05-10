using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using viki_01.Contexts;
using viki_01.Entities;
using viki_01.Extensions;

namespace viki_01.Controllers;

[ApiController]
[Route("[controller]")]
public class RatingController(WikiHostingSqlServerContext context, ILoggerFactory loggerFactory) : ControllerBase
{
    private readonly ILogger<RatingController> logger =
        loggerFactory.CreateLogger<RatingController>();
    
    [HttpPost($"{nameof(Like)}/{{pageId:int}}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize]
    public async Task<IActionResult> Like([FromRoute] int pageId) {
        logger.LogInformation("Like called with pageId: {pageId}", pageId);
        
        var page = await context.Pages.Where(p => p.Id == pageId).Include(p => p.UserRatings)
            .FirstOrDefaultAsync();
        
        if (page == null) {
            logger.LogWarning("Page with ID {pageId} not found", pageId);
            return NotFound();
        }

        var userId = HttpContext.User.GetId();
        var userRatings = page.UserRatings.Where(r => r.UserId == userId).ToList();

        if (userRatings.Any())
        {
            logger.LogInformation("User with ID {userId} already rated page with ID {pageId}. Old ratings will be removed.", userId, pageId);

            foreach (var rating in userRatings)
            {
                context.Ratings.Remove(rating);
            }

            await context.SaveChangesAsync();
        }

        var newRating = new Rating
        {
            UserId = userId,
            PageId = pageId,
            NumberOfLikes = 1,
            NumberOfDislikes = 0
        };
        
        page.UserRatings.Add(newRating);

        await context.SaveChangesAsync();

        logger.LogInformation("User with ID {userId} liked page with ID {pageId}", userId, pageId);
        return CreatedAtAction(nameof(Like), new { pageId }, newRating);
    }
    
    [HttpPost($"{nameof(Dislike)}/{{pageId:int}}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize]
    public async Task<IActionResult> Dislike([FromRoute] int pageId) {
        logger.LogInformation("Dislike called with pageId: {pageId}", pageId);
        
        var page = await context.Pages.Where(p => p.Id == pageId).Include(p => p.UserRatings)
            .FirstOrDefaultAsync();
        
        if (page == null) {
            logger.LogWarning("Page with ID {pageId} not found", pageId);
            return NotFound();
        }

        var userId = HttpContext.User.GetId();
        var userRatings = page.UserRatings.Where(r => r.UserId == userId).ToList();

        if (userRatings.Any())
        {
            logger.LogInformation("User with ID {userId} already rated page with ID {pageId}. Old ratings will be removed.", userId, pageId);

            foreach (var rating in userRatings)
            {
                context.Ratings.Remove(rating);
            }

            await context.SaveChangesAsync();
        }

        var newRating = new Rating
        {
            UserId = userId,
            PageId = pageId,
            NumberOfLikes = 0,
            NumberOfDislikes = 1
        };
        
        page.UserRatings.Add(newRating);

        await context.SaveChangesAsync();

        logger.LogInformation("User with ID {userId} disliked page with ID {pageId}", userId, pageId);
        return CreatedAtAction(nameof(Dislike), new { pageId }, newRating);
    }
    
    [HttpDelete($"{nameof(Clear)}/{{pageId:int}}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize]
    public async Task<IActionResult> Clear([FromRoute] int pageId) {
        logger.LogInformation("RemoveRating called with pageId: {pageId}", pageId);
        
        var page = await context.Pages.Where(p => p.Id == pageId).Include(p => p.UserRatings)
            .FirstOrDefaultAsync();
        
        if (page == null) {
            logger.LogWarning("Page with ID {pageId} not found", pageId);
            return NotFound();
        }

        var userId = HttpContext.User.GetId();
        var userRatings = page.UserRatings.Where(r => r.UserId == userId).ToList();

        if (!userRatings.Any())
        {
            logger.LogWarning("User with ID {userId} did not rate page with ID {pageId}", userId, pageId);
            return BadRequest();
        }

        foreach (var rating in userRatings)
        {
            context.Ratings.Remove(rating);
        }

        await context.SaveChangesAsync();

        logger.LogInformation("User with ID {userId} removed rating from page with ID {pageId}", userId, pageId);
        return Ok();
    }
}