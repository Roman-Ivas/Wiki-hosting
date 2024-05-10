using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using viki_01.Contexts;
using viki_01.Entities;
using viki_01.Extensions;
using viki_01.Models.Dto;

namespace viki_01.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TemplateController(
        WikiHostingSqlServerContext context,
        ILogger<TemplateController> logger)
        : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetTemplates([FromQuery] string? search = null)
        {
            logger.LogInformation("GetTemplates called");

            IQueryable<Template> templates = context.Templates;

            if (!string.IsNullOrEmpty(search))
            {
                templates = templates.Where(t => t.Name.Contains(search));
            }

            var templateList = await templates.ToListAsync();
            return Ok(templateList);
        }
        
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetTemplate([FromRoute] int id)
        {
            logger.LogInformation("GetTemplate called with ID: {id}", id);

            var template = await context.Templates.FindAsync(id);
            if (template is null)
            {
                logger.LogWarning("Template with ID {id} not found", id);
                return NotFound();
            }

            return Ok(template);
        }

        [HttpDelete("{id:int}")]
        [Authorize("TemplateOwner")]
        public async Task<IActionResult> DeleteTemplate(int id)
        {
            logger.LogInformation("DeleteTemplate called with ID: {id}", id);

            var template = await context.Templates.FindAsync(id);
            if (template == null)
            {
                logger.LogWarning("Template with ID {id} not found", id);
                return NotFound();
            }
            context.Templates.Remove(template);
            await context.SaveChangesAsync();

            logger.LogInformation("Template with ID {id} deleted successfully", id);
            return NoContent();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateTemplate(TemplateDto templateDto)
        {
            logger.LogInformation("CreateTemplate called with template: {@templateDto}", templateDto);

            var authorId = HttpContext.User.GetId();
            var template = new Template
            {
                Name = templateDto.Name,
                AuthorId = authorId,
                Html = templateDto.Html,
                Css = templateDto.Css,
                Js = templateDto.Js,
                Variables = templateDto.Variables,
                IsPublic = templateDto.IsPublic
            };

            await context.Templates.AddAsync(template);
            await context.SaveChangesAsync();

            logger.LogInformation("Template created successfully with ID: {id}", template.Id);
            return CreatedAtAction(nameof(GetTemplates), new { id = template.Id }, template);
        }

        [HttpPut("{id:int}")]
        [Authorize("TemplateOwner")]
        public async Task<IActionResult> UpdateTemplate(int id, TemplateDto updateTemplateDto)
        {
            logger.LogInformation("UpdateTemplate called with ID: {id} and updated template: {@updateTemplateDto}", id, updateTemplateDto);
            
            
            var existingTemplate = await context.Templates.FindAsync(id);
            if (existingTemplate == null)
            {
                logger.LogWarning("Template with ID {id} not found", id);
                return NotFound();
            }

            existingTemplate.Name = updateTemplateDto.Name;
            existingTemplate.Html = updateTemplateDto.Html;
            existingTemplate.Css = updateTemplateDto.Css;
            existingTemplate.Js = updateTemplateDto.Js;
            existingTemplate.Variables = updateTemplateDto.Variables;
            existingTemplate.IsPublic = updateTemplateDto.IsPublic;

            await context.SaveChangesAsync();

            logger.LogInformation("Template with ID {id} updated successfully", id);
            return NoContent();
        }
    }
}
