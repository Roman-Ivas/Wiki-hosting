using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class TemplateController : ControllerBase
    {
        private readonly WikiHostingSqlServerContext _context;
        private readonly ILogger<TemplateController> _logger;

        public TemplateController(WikiHostingSqlServerContext context, ILogger<TemplateController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("{id:int?}")]
        public async Task<IActionResult> GetTemplate(int? id, [FromQuery] string search)
        {
            _logger.LogInformation("GetTemplate called with ID: {id}", id);

            IQueryable<Template> templates = _context.Templates;

            if (id.HasValue)
            {
                var template = await templates.FirstOrDefaultAsync(t => t.Id == id);
                if (template == null)
                {
                    _logger.LogWarning("Template with ID {id} not found", id);
                    return NotFound();
                }
                return Ok(template);
            }

            if (!string.IsNullOrEmpty(search))
            {
                templates = templates.Where(t => t.Name.Contains(search));
            }

            var templateList = await templates.ToListAsync();
            return Ok(templateList);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteTemplate(int id)
        {
            _logger.LogInformation("DeleteTemplate called with ID: {id}", id);

            var template = await _context.Templates.FindAsync(id);
            if (template == null)
            {
                _logger.LogWarning("Template with ID {id} not found", id);
                return NotFound();
            }
            _context.Templates.Remove(template);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Template with ID {id} deleted successfully", id);
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTemplate(Template template)
        {
            _logger.LogInformation("CreateTemplate called with template: {@template}", template);

            _context.Templates.Add(template);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Template created successfully with ID: {id}", template.Id);
            return CreatedAtAction(nameof(GetTemplate), new { id = template.Id }, template);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateTemplate(int id, Template updateTemplate)
        {
            _logger.LogInformation("UpdateTemplate called with ID: {id} and updated template: {@updateTemplate}", id, updateTemplate);

            if (id != updateTemplate.Id)
            {
                _logger.LogWarning("Invalid ID provided in the request");
                return BadRequest();
            }

            var existingTemplate = await _context.Templates.FindAsync(id);
            if (existingTemplate == null)
            {
                _logger.LogWarning("Template with ID {id} not found", id);
                return NotFound();
            }

            existingTemplate.Name = updateTemplate.Name;
            existingTemplate.AuthorId = updateTemplate.AuthorId;
            existingTemplate.Html = updateTemplate.Html;
            existingTemplate.Css = updateTemplate.Css;
            existingTemplate.Js = updateTemplate.Js;
            existingTemplate.Variables = updateTemplate.Variables;
            existingTemplate.IsPublic = updateTemplate.IsPublic;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Template with ID {id} updated successfully", id);
            return NoContent();
        }
    }
}
