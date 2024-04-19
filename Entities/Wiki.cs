using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace viki_01.Entities
{
    [SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
    [SuppressMessage("ReSharper", "EntityFramework.ModelValidation.CircularDependency")] // Can be solved by correct JSON serialization configuration
    public class Wiki
    {
        public int Id { get; set; }
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = null!;
        public bool IsArchived { get; set; }

        [StringLength(512, MinimumLength = 1)]
        public string BackgroundImagePath { get; set; } = null!;

        [StringLength(512, MinimumLength = 1)]
        public string MainWikiImagePath { get; set; } = null!;

        public IList<Link> MainLinks { get; set; } = new List<Link>();
        public IList<Topic> Topics { get; set; } = new List<Topic>();
        public IList<Page> Pages { get; set; } = new List<Page>();
        public IList<Contributor> Contributors { get; set; } = new List<Contributor>();
    }
}
