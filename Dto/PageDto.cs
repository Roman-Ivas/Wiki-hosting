using System.ComponentModel.DataAnnotations;

namespace viki_01.Dto;

public class PageDto
{
    public int WikiId { get; set; }
    public int AuthorId { get; set; } = default!;
    
    [StringLength(int.MaxValue, MinimumLength = 1)]
    public string RawHtml { get; set; } = null!;
    [StringLength(int.MaxValue, MinimumLength = 1)]
    public string ProcessedHtml { get; set; } = null!;
}