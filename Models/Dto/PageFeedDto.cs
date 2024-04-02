namespace viki_01.Models.Dto;

public class PageFeedDto
{
    public int Id { get; set; }

    public int WikiId { get; set; }

    public AuthorDto Author { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public DateTime EditedAt { get; set; }

    public string Title { get; set; } = null!;
    public string ImagePath { get; set; } = null!;

    public int NumberOfLikes { get; set; }
    public int NumberOfComments { get; set; }
}