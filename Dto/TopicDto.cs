using System.ComponentModel.DataAnnotations;

namespace viki_01.Dto;

public class TopicDto
{
    public int Id { get; set; }
    
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = null!;
}