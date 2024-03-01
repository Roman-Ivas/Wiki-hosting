using viki_01.Dto;
using viki_01.Entities;

namespace viki_01.Services;

public class TopicToTopicDtoMapper : IMapper<Topic, TopicDto>
{
    public TopicDto Map(Topic source)
    {
        return new TopicDto
        {
            Id = source.Id,
            Name = source.Name
        };
    }

    public Topic Map(TopicDto destination)
    {
        return new Topic
        {
            Id = destination.Id,
            Name = destination.Name
        };
    }

    public void Map(Topic source, Topic destination)
    {
        destination.Name = source.Name;
        destination.InterestedUsers = source.InterestedUsers;
        destination.RelevantWikis = source.RelevantWikis;
    }
    
}