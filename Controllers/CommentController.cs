using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using viki_01.Contexts;
using viki_01.Entities;

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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetComment([FromRoute] int id)
        {
            _logger.LogInformation("GetComment called with ID: {id}", id);

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                _logger.LogWarning("Comment with ID {id} not found", id);
                return NotFound();
            }

            _logger.LogInformation("Comment with ID {id} found and returned", id);
            return Ok(comment);
        }

        [HttpPost("{articleId:int}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateComment([FromRoute] int articleId, [FromBody] Comment comment)
        {
            _logger.LogInformation("CreateComment called with article ID: {articleId} and comment: {@comment}", articleId, comment);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state");
                return BadRequest(ModelState);
            }

            comment.PageId = articleId;
            comment.PostedAt = DateTime.Now;
            comment.EditedAt = DateTime.Now;

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Comment created successfully with ID: {id}", comment.Id);
            return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, comment);
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
