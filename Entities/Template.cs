using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace viki_01.Entities
{
    [SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
    [SuppressMessage("ReSharper", "EntityFramework.ModelValidation.CircularDependency")] // Can be solved by correct JSON serialization configuration
    public class Template
    {
        public int Id { get; set; }

        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = null!;

        [StringLength(450, MinimumLength = 1)]
        public int AuthorId { get; set; } = default!;
        public User Author { get; set; } = null!;

        [StringLength(int.MaxValue, MinimumLength = 1)]
        public string Html { get; set; } = null!;
        [StringLength(int.MaxValue)]
        public string? Css { get; set; }
        [StringLength(int.MaxValue)]
        public string? Js { get; set; }
        [StringLength(int.MaxValue)]
        public string? Variables { get; set; }
        
        [StringLength(450, MinimumLength = 1)]
        public string ImagePath { get; set; } = null!;

        public bool IsPublic { get; set; }
    }
}
