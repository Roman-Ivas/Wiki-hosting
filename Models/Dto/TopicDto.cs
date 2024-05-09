using System.ComponentModel.DataAnnotations;
using viki_01.Entities;

namespace viki_01.Models.Dto;

public class TopicDto
{
    public int Id { get; set; }
    
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = null!;
    [StringLength(512, MinimumLength = 1)]
    public string topicImage { get; set; } = null!;
}