using Firebase.Storage;

namespace viki_01.Services;

public class FirebaseFileSaver(string bucket) : IFileSaver
{
    private readonly FirebaseStorage storage = new(bucket);

    public async Task<string> SaveFileAsync(IFormFile file, string? bucketName = null)
    {
        await using var stream = file.OpenReadStream();
        var fileExtension = file.FileName.Split('.').Last();
        var fileName = Guid.NewGuid().ToString("N") + '.' + fileExtension;
        var filePath = !string.IsNullOrWhiteSpace(bucketName) ? bucketName + '/' + fileName : fileName;
        await storage.Child(filePath).PutAsync(stream);

        return await storage.Child(filePath).GetDownloadUrlAsync();
    }
}
