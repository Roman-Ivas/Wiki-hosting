using viki_01.Entities;
using viki_01.Models.Dto;
using viki_01.Services;

public class TopicToTopicDtoMapper : IMapper<Topic, TopicDto>
{
    public TopicDto Map(Topic source)
    {
        return new TopicDto
        {
            Id = source.Id,
            Name = source.Name,
            topicImage = source.topicImage 
        };
    }

    public Topic Map(TopicDto destination)
    {
        return new Topic
        {
            Id = destination.Id,
            Name = destination.Name,
            topicImage = destination.topicImage 
        };
    }

    public void Map(Topic source, Topic destination)
    {
        destination.Name = source.Name;
        destination.topicImage = source.topicImage;
        destination.InterestedUsers = source.InterestedUsers;
        destination.RelevantWikis = source.RelevantWikis;
    }
}
