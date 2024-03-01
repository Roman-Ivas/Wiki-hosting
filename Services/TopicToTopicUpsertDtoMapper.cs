using viki_01.Dto;
using viki_01.Entities;

namespace viki_01.Services;

public class TopicToTopicUpsertDtoMapper : IMapper<Topic, TopicUpsertDto>
{
    public TopicUpsertDto Map(Topic source)
    {
        return new TopicUpsertDto
        {
            Name = source.Name
        };
    }

    public Topic Map(TopicUpsertDto destination)
    {
        return new Topic
        {
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