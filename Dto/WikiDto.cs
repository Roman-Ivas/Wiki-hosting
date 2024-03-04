using System.ComponentModel.DataAnnotations;
using viki_01.Entities;

namespace viki_01.Dto;

public class WikiDto
{
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = null!;
    public bool IsArchived { get; set; }
}