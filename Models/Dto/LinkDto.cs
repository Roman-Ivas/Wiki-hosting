using System.ComponentModel.DataAnnotations;

namespace viki_01.Models.Dto;

public class LinkDto
{
    public int Id { get; set; }
    
    [StringLength(512, MinimumLength = 1)]
    public string? Url { get; set; }
    [StringLength(128, MinimumLength = 1)]
    public string? Title { get; set; }
}