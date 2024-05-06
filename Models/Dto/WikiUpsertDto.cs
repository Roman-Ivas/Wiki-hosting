using System.ComponentModel.DataAnnotations;

namespace viki_01.Models.Dto;

public class WikiUpsertDto
{
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = null!;
    
    [StringLength(512)]
    public string BackgroundImagePath { get; set; } = null!;

    [StringLength(512)] 
    public string MainWikiImagePath { get; set; } = null!;

    [StringLength(128)]
    public string Topic { get; set; } = null!;

    public IEnumerable<LinkDto> MainLinks { get; set; } = new List<LinkDto>();
    public IEnumerable<ContributorDto> Contributors { get; set; } = new List<ContributorDto>();
}