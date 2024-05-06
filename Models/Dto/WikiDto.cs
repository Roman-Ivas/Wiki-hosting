using System.ComponentModel.DataAnnotations;

namespace viki_01.Models.Dto;

public class WikiDto
{
    public int Id { get; set; }
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = null!;
    public bool IsArchived { get; set; }
    public int NumberOfPages { get; set; }
    
    [StringLength(512, MinimumLength = 1)]
    public string BackgroundImagePath { get; set; } = null!;

    [StringLength(512, MinimumLength = 1)]
    public string MainWikiImagePath { get; set; } = null!;

    public IEnumerable<LinkDto> MainLinks { get; set; } = new List<LinkDto>();
    public IEnumerable<ContributorDto> Contributors { get; set; } = new List<ContributorDto>();
}