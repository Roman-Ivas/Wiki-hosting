using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using viki_01.Contexts;
using viki_01.Entities;
using viki_01.Models;
using viki_01.Models.Dto;

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
        [HttpGet]
        public async Task<IActionResult> GetTemplates([FromQuery] string? search = null)
        {
            _logger.LogInformation("GetTemplates called");

            IQueryable<Template> templates = _context.Templates;

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
        public async Task<IActionResult> CreateTemplate(TemplateDto templateDto)
        {
            _logger.LogInformation("CreateTemplate called with template: {@templateDto}", templateDto);

            var template = new Template
            {
                Name = templateDto.Name,
                AuthorId = templateDto.AuthorId,
                Html = templateDto.Html,
                Css = templateDto.Css,
                Js = templateDto.Js,
                Variables = templateDto.Variables,
                IsPublic = templateDto.IsPublic
            };

            _context.Templates.Add(template);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Template created successfully with ID: {id}", template.Id);
            return CreatedAtAction(nameof(GetTemplates), new { id = template.Id }, template);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateTemplate(int id, TemplateDto updateTemplateDto)
        {
            _logger.LogInformation("UpdateTemplate called with ID: {id} and updated template: {@updateTemplateDto}", id, updateTemplateDto);

            var existingTemplate = await _context.Templates.FindAsync(id);
            if (existingTemplate == null)
            {
                _logger.LogWarning("Template with ID {id} not found", id);
                return NotFound();
            }

            existingTemplate.Name = updateTemplateDto.Name;
            existingTemplate.AuthorId = updateTemplateDto.AuthorId;
            existingTemplate.Html = updateTemplateDto.Html;
            existingTemplate.Css = updateTemplateDto.Css;
            existingTemplate.Js = updateTemplateDto.Js;
            existingTemplate.Variables = updateTemplateDto.Variables;
            existingTemplate.IsPublic = updateTemplateDto.IsPublic;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Template with ID {id} updated successfully", id);
            return NoContent();
        }
    }
}
