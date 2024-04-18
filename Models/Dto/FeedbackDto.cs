using System.ComponentModel.DataAnnotations;

namespace viki_01.Models.Dto;

public class FeedbackDto
{
    public int UserId { get; set; } = default!;
    [StringLength(500, MinimumLength = 1)]
    public string Text { get; set; } = null!;
}