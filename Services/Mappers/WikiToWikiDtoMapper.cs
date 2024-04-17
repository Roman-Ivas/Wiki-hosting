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
            IsArchived = original.IsArchived,
            NumberOfPages = original.Pages.Count
        };
    }

    public Wiki Map(WikiDto transformed)
    {
        return new Wiki
        {
            Name = transformed.Name,
            IsArchived = transformed.IsArchived,
            Pages = new List<Page>(transformed.NumberOfPages)
        };
    }

    public void Map(Wiki source, Wiki destination)
    {
        destination.Name = source.Name;
        destination.IsArchived = source.IsArchived;
        destination.Pages = source.Pages;
    }
}