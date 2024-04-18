using System.ComponentModel.DataAnnotations;

namespace viki_01.Models.Dto;

public class WikiUpsertDto
{
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = null!;
    
    [StringLength(512, MinimumLength = 1)]
    public string BackgroundImagePath { get; set; } = null!;
    
    public IEnumerable<LinkDto> MainLinks { get; set; } = new List<LinkDto>();
}