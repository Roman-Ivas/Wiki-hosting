using viki_01.Dto;
using viki_01.Entities;

namespace viki_01.Services;

public class WikiToWikiUpsertDtoMapper : IMapper<Wiki, WikiUpsertDto>
{
    public WikiUpsertDto Map(Wiki original)
    {
        return new WikiUpsertDto
        {
            Name = original.Name
        };
    }

    public Wiki Map(WikiUpsertDto transformed)
    {
        return new Wiki
        {
            Name = transformed.Name
        };
    }

    public void Map(Wiki source, Wiki destination)
    {
        destination.Name = source.Name;
    }
}