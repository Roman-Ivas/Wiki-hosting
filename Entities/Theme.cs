using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace viki_01.Entities
{
    [SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
    [SuppressMessage("ReSharper", "EntityFramework.ModelValidation.CircularDependency")] // Can be solved by correct JSON serialization configuration
    public class Theme
    {
        public int Id { get; set; }
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = null!;

        [StringLength(450, MinimumLength = 1)]
        public int AuthorId { get; set; } = default!;
        public User Author { get; set; } = null!;

        [StringLength(int.MaxValue, MinimumLength = 1)]
        public string Css { get; set; } = null!;

        public bool IsPublic { get; set; }
    }
}
