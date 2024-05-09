using System.ComponentModel.DataAnnotations;

namespace viki_01.Models.Dto;

public class ContributorDto
{
    public int Id { get; set; }
    [StringLength(256)] public string UserName { get; set; } = null!;
    public int UserId { get; set; }
    public int WikiId { get; set; }
    public int ContributorRoleId { get; set; }
}