using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace viki_01.Entities
{
    [SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
    [SuppressMessage("ReSharper", "EntityFramework.ModelValidation.CircularDependency")] // Can be solved by correct JSON serialization configuration
    public class ContributorRole
    {
        public int Id { get; set; }
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = null!;

        public IList<Contributor> Contributors { get; set; } = null!;
    }
}
