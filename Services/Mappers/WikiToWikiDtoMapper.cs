using viki_01.Entities;
using viki_01.Models.Dto;

namespace viki_01.Services.Mappers;

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