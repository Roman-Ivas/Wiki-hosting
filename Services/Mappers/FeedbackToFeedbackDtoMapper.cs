using viki_01.Entities;
using viki_01.Models.Dto;

namespace viki_01.Services.Mappers;

public class FeedbackToFeedbackDtoMapper : IMapper<Feedback, FeedbackDto>
{
    public FeedbackDto Map(Feedback original)
    {
        return new FeedbackDto
        {
            Text = original.Text,
            UserId = original.UserId
        };
    }

    public Feedback Map(FeedbackDto transformed)
    {
        return new Feedback
        {
            Text = transformed.Text,
            UserId = transformed.UserId
        };
    }

    public void Map(Feedback source, Feedback destination)
    {
        destination.Text = source.Text;
        destination.UserId = source.UserId;
    }
}