using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace viki_01.Entities
{
    [SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
    [SuppressMessage("ReSharper", "EntityFramework.ModelValidation.CircularDependency")] // Can be solved by correct JSON serialization configuration

    public class Comment
    {
        public int Id { get; set; }

        public int PageId { get; set; }
        public Page Page { get; set; } = null!;

        [StringLength(450, MinimumLength = 1)]
        public int AuthorId { get; set; } = default!;
        public User Author { get; set; } = null!;

        [StringLength(500, MinimumLength = 1)]
        public string Text { get; set; } = null!;
        public DateTime PostedAt { get; set; }
        public DateTime EditedAt { get; set; }

        // Self-referencing relationship
        public int? ParentCommentId { get; set; }
        public Comment? ParentComment { get; set; }
    }
}
