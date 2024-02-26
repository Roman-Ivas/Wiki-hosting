using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace viki_01.Entities
{
    [SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
    [SuppressMessage("ReSharper", "EntityFramework.ModelValidation.CircularDependency")] // Can be solved by correct JSON serialization configuration
    public class Report
    {
        public int Id { get; set; }

        [StringLength(450, MinimumLength = 1)]
        public int UserId { get; set; } = default!;
        public User User { get; set; } = null!;

        [StringLength(256, MinimumLength = 1)]
        public string ReportedContentUrl { get; set; } = null!;
        public DateTime PostedAt { get; set; }
        [StringLength(1000, MinimumLength = 1)]
        public string Text { get; set; } = null!;

        [StringLength(500, MinimumLength = 1)]
        public string? Result { get; set; }
    }
}
