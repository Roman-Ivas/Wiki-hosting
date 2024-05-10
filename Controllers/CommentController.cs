using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using viki_01.Contexts;
using viki_01.Entities;
using viki_01.Extensions;
using viki_01.Models.Dto;

namespace viki_01.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly WikiHostingSqlServerContext _context;
        private readonly ILogger<CommentController> _logger;

        public CommentController(WikiHostingSqlServerContext context, ILogger<CommentController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetComment([FromRoute] int id)
        {
            _logger.LogInformation("GetComment called with ID: {id}", id);

            var comment = await _context.Comments.Where(c => c.Id == id).Include(c => c.Author)
                .FirstOrDefaultAsync();
            
            if (comment == null)
            {
                _logger.LogWarning("Comment with ID {id} not found", id);
                return NotFound();
            }

            _logger.LogInformation("Comment with ID {id} found and returned", id);
            return Ok(comment);
        }
        
        [HttpGet($"{nameof(GetPageComments)}/{{pageId:int}}")]
        public async Task<IActionResult> GetPageComments([FromRoute] int pageId)
        {
            _logger.LogInformation("GetPageComments called with pageId ID: {pageId}", pageId);

            var comments = await _context.Comments.Where(c => c.PageId == pageId).Include(c => c.Author)
                .ToListAsync();
            
            _logger.LogInformation("Comments for page ID {pageId} found and returned", pageId);
            return Ok(comments);
        }

        [HttpPost("{articleId:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateComment([FromRoute] int articleId, [FromBody] CommentDto commentDto)
        {
            _logger.LogInformation("CreateComment called with article ID: {articleId} and comment: {@commentDto}", articleId, commentDto);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state");
                return BadRequest(ModelState);
            }

            var comment = new Comment
            {
                PageId = articleId,
                AuthorId = HttpContext.User.GetId(),
                Text = commentDto.Text,
                PostedAt = DateTime.Now,
                EditedAt = DateTime.Now
            };

            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Comment created successfully with ID: {id}", comment.Id);
            return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, await _context.Comments.Where(c => c.Id == comment.Id).Include(c => c.Author).FirstOrDefaultAsync());
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateComment([FromRoute] int id, [FromBody] Comment updatedComment)
        {
            _logger.LogInformation("UpdateComment called with ID: {id} and updated comment: {@updatedComment}", id, updatedComment);

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                _logger.LogWarning("Comment with ID {id} not found", id);
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state");
                return BadRequest(ModelState);
            }

            comment.Text = updatedComment.Text;
            comment.EditedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Comment with ID {id} updated successfully", id);
            return Ok(comment);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteComment([FromRoute] int id)
        {
            _logger.LogInformation("DeleteComment called with ID: {id}", id);

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                _logger.LogWarning("Comment with ID {id} not found", id);
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Comment with ID {id} deleted successfully", id);
            return NoContent();
        }
    }
}
