using viki_01.Entities;
using viki_01.Models.Dto;

namespace viki_01.Services.Mappers;

public class WikiToWikiDtoMapper : IMapper<Wiki, WikiDto>
{
    public WikiDto Map(Wiki original)
    {
        return new WikiDto
        {
            Id = original.Id,
            Name = original.Name,
            MainWikiImagePath = original.MainWikiImagePath, 
            IsArchived = original.IsArchived,
            NumberOfPages = original.Pages.Count
        };
    }

    public Wiki Map(WikiDto transformed)
    {
        return new Wiki
        {
            Id = transformed.Id,
            Name = transformed.Name,
            MainWikiImagePath = transformed.MainWikiImagePath, 
            IsArchived = transformed.IsArchived,
            Pages = new List<Page>(transformed.NumberOfPages)
        };
    }

    public void Map(Wiki source, Wiki destination)
    {
        destination.Id = source.Id;
        destination.Name = source.Name;
        destination.MainWikiImagePath = source.MainWikiImagePath;
        destination.IsArchived = source.IsArchived;
        destination.Pages = source.Pages;
    }
}