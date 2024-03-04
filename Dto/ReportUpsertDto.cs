using System.ComponentModel.DataAnnotations;

namespace viki_01.Dto;

public class ReportUpsertDto
{
    public int UserId { get; set; } = default!;
    
    [StringLength(256, MinimumLength = 1)]
    public string ReportedContentUrl { get; set; } = null!;
    
    [StringLength(1000, MinimumLength = 1)]
    public string Text { get; set; } = null!;
}