using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace viki_01.Entities
{
    [SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
    [SuppressMessage("ReSharper", "EntityFramework.ModelValidation.CircularDependency")] // Can be solved by correct JSON serialization configuration

    // Any additional content on the page (photos, videos, etc.) that the user has inserted from their local storage,
    // rather than using, for example, a link from the internet
    // Should be deleted automatically when the page is deleted, so we need to store the path to the file
    public class MediaContent
    {
        public int Id { get; set; }
        [StringLength(512, MinimumLength = 1)]
        public string Path { get; set; } = null!;
        [StringLength(256, MinimumLength = 1)]
        public string FileName { get; set; } = null!;

        public int PageId { get; set; }
        public Page Page { get; set; } = null!;
    }
}
