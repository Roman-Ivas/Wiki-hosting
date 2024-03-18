using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using viki_01.Contexts;
using viki_01.Entities;
using viki_01.Models.Dto;

namespace viki_01.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ThemeController : ControllerBase
    {
        private readonly WikiHostingSqlServerContext _context;
        private readonly ILogger<ThemeController> _logger;

        public ThemeController(WikiHostingSqlServerContext context, ILogger<ThemeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("{id:int?}")]
        public async Task<IActionResult> GetThemes(int? id, [FromQuery] string search)
        {
            _logger.LogInformation("GetThemes called with ID: {id}", id);

            IQueryable<Theme> themes = _context.Themes;

            if (id.HasValue)
            {
                themes = themes.Where(t => t.Id == id);
            }

            if (!string.IsNullOrEmpty(search))
            {
                themes = themes.Where(t => t.Name.Contains(search));
            }

            return Ok(await themes.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CreateTheme(ThemeDto themeDto)
        {
            _logger.LogInformation("CreateTheme called with theme: {@themeDto}", themeDto);

            var theme = new Theme
            {
                Name = themeDto.Name,
                AuthorId = themeDto.AuthorId,
                Css = themeDto.Css,
                IsPublic = themeDto.IsPublic
            };

            _context.Themes.Add(theme);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Theme created successfully with ID: {id}", theme.Id);
            return CreatedAtAction(nameof(GetThemes), new { id = theme.Id }, theme);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateTheme(int id, ThemeDto updateThemeDto)
        {
            _logger.LogInformation("UpdateTheme called with ID: {id} and updated theme: {@updateThemeDto}", id, updateThemeDto);

            var existingTheme = await _context.Themes.FindAsync(id);
            if (existingTheme == null)
            {
                _logger.LogWarning("Theme with ID {id} not found", id);
                return NotFound();
            }

            existingTheme.Name = updateThemeDto.Name;
            existingTheme.AuthorId = updateThemeDto.AuthorId;
            existingTheme.Css = updateThemeDto.Css;
            existingTheme.IsPublic = updateThemeDto.IsPublic;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Theme with ID {id} updated successfully", id);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteTheme(int id)
        {
            _logger.LogInformation("DeleteTheme called with ID: {id}", id);

            var theme = await _context.Themes.FindAsync(id);
            if (theme == null)
            {
                _logger.LogWarning("Theme with ID {id} not found", id);
                return NotFound();
            }

            _context.Themes.Remove(theme);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Theme with ID {id} deleted successfully", id);
            return NoContent();
        }
    }
}
