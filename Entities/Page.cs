using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace viki_01.Entities
{
    [SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
    [SuppressMessage("ReSharper", "EntityFramework.ModelValidation.CircularDependency")] // Can be solved by correct JSON serialization configuration
    public class Page
    {
        public int Id { get; set; }

        public int WikiId { get; set; }
        public Wiki Wiki { get; set; } = null!;

        [StringLength(450, MinimumLength = 1)]
        public int AuthorId { get; set; } = default!;
        public User Author { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime EditedAt { get; set; }

        [StringLength(int.MaxValue, MinimumLength = 1)]
        public string RawHtml { get; set; } = null!;
        [StringLength(int.MaxValue, MinimumLength = 1)]
        public string ProcessedHtml { get; set; } = null!;

        public IList<MediaContent> MediaContents { get; set; } = new List<MediaContent>();
        public IList<Rating> UserRatings { get; set; } = new List<Rating>();
        public IList<Comment> Comments { get; set; } = new List<Comment>();
    }
}
