namespace viki_01.Services;

public interface IFileSaver
{
    Task<string> SaveFileAsync(IFormFile file, string? bucketName = null);
}