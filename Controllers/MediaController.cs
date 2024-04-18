using Microsoft.AspNetCore.Mvc;
using viki_01.Contexts;
using viki_01.Entities;
using viki_01.Services;
using viki_01.Utils;

namespace viki_01.Controllers;

[ApiController]
[Route("/[controller]")]
public class MediaController([FromServices] IFileSaver fileSaver, WikiHostingSqlServerContext context) : ControllerBase
{
    [HttpPost("SaveImage")]
    public async Task<IActionResult> SaveImage(IFormFile image)
    {
        if (image.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        try
        {
            var url = await fileSaver.SaveFileAsync(image, "images");
            var mediaContent = new MediaContent
            {
                Path = url
            };

            await context.MediaContents.AddAsync(mediaContent);
            return Ok(url);
        }
        catch (Exception exception)
        {
            // Log the exception as needed
            return Problem("An error occured while saving the file.");
        }
    }
}
