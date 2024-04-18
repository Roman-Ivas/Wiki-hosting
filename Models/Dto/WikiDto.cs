using System.ComponentModel.DataAnnotations;

namespace viki_01.Models.Dto;

public class WikiDto
{
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = null!;
    public bool IsArchived { get; set; }
    public int NumberOfPages { get; set; }
}