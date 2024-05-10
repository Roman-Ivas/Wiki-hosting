using System.ComponentModel.DataAnnotations;
using viki_01.Entities;

namespace viki_01.Models.Dto;

public class PageDto
{
    public int Id { get; set; }
    public int WikiId { get; set; }
    public int AuthorId { get; set; } = default!;
    
    [StringLength(int.MaxValue, MinimumLength = 1)]
    public string RawHtml { get; set; } = null!;
    [StringLength(int.MaxValue, MinimumLength = 1)]
    public string ProcessedHtml { get; set; } = null!;
    public IEnumerable<Rating> UserRatings { get; set; } = new List<Rating>();
}