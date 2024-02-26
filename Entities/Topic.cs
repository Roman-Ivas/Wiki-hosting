using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace viki_01.Entities
{
    [SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
    [SuppressMessage("ReSharper", "EntityFramework.ModelValidation.CircularDependency")] // Can be solved by correct JSON serialization configuration
    public class Topic
    {
        public int Id { get; set; }
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = null!;
        public IList<User> InterestedUsers { get; set; } = new List<User>();
        public IList<Wiki> RelevantWikis { get; set; } = new List<Wiki>();
    }
}
