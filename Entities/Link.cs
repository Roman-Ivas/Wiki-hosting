using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace viki_01.Entities;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.CircularDependency")] // Can be solved by correct JSON serialization configuration
public class Link
{
    public int Id { get; set; }
    
    [StringLength(512, MinimumLength = 1)]
    public string Url { get; set; } = null!;
    [StringLength(128, MinimumLength = 1)]
    public string Title { get; set; } = null!;
    
    public int WikiId { get; set; }
    public Wiki Wiki { get; set; } = null!;
}