using System.ComponentModel.DataAnnotations;
using viki_01.Entities;

namespace viki_01.Models.Dto;

public class WikiDto
{
    public int Id { get; set; }
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = null!;
    public bool IsArchived { get; set; }
    public int NumberOfPages { get; set; }
    [StringLength(512, MinimumLength = 1)]
    public string MainWikiImagePath { get; set; } = null!;

}