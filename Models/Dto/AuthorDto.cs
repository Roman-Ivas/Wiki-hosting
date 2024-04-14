namespace viki_01.Models.Dto;

public class AuthorDto
{
    public int Id { get; set; }
    public required string UserName { get; set; }
    public required string AvatarPath { get; set; }
}