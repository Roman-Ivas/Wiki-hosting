using System.ComponentModel.DataAnnotations;

namespace viki_01.Dto;

public class PageUpsertDto
{
    [StringLength(int.MaxValue, MinimumLength = 1)]
    public string RawHtml { get; set; } = null!;
    [StringLength(int.MaxValue, MinimumLength = 1)]
    public string ProcessedHtml { get; set; } = null!;
}