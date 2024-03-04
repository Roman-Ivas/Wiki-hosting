using viki_01.Dto;
using viki_01.Entities;

namespace viki_01.Services;

public class WikiToWikiDtoMapper : IMapper<Wiki, WikiDto>
{
    public WikiDto Map(Wiki original)
    {
        return new WikiDto
        {
            Name = original.Name,
            IsArchived = original.IsArchived
        };
    }

    public Wiki Map(WikiDto transformed)
    {
        return new Wiki
        {
            Name = transformed.Name,
            IsArchived = transformed.IsArchived
        };
    }

    public void Map(Wiki source, Wiki destination)
    {
        destination.Name = source.Name;
        destination.IsArchived = source.IsArchived;
    }
}