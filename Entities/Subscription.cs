using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace viki_01.Entities
{
    [SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
    [SuppressMessage("ReSharper", "EntityFramework.ModelValidation.CircularDependency")] // Can be solved by correct JSON serialization configuration
    public class Subscription
    {
        public int Id { get; set; }
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = null!;
        [StringLength(500, MinimumLength = 1)]
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public int DurationDays { get; set; }

        public IList<UserSubscription> Users { get; set; } = new List<UserSubscription>();
    }
}
