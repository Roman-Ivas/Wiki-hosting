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
            IsArchived = original.IsArchived,
            NumberOfPages = original.Pages.Count,
            BackgroundImagePath = original.BackgroundImagePath,
            MainWikiImagePath = original.MainWikiImagePath,
            MainLinks = original.MainLinks.Select(link => new LinkDto { Title = link.Title, Url = link.Url, Id = link.Id }),
            Contributors = original.Contributors.Select(contributor => new ContributorDto { UserId = contributor.UserId, WikiId = contributor.WikiId, ContributorRoleId = contributor.ContributorRoleId })
        };
    }

    public Wiki Map(WikiDto transformed)
    {
        return new Wiki
        {
            Id = transformed.Id,
            Name = transformed.Name,
            IsArchived = transformed.IsArchived,
            Pages = new List<Page>(transformed.NumberOfPages),
            BackgroundImagePath = transformed.BackgroundImagePath,
            MainWikiImagePath = transformed.MainWikiImagePath,
            MainLinks = transformed.MainLinks.Select(linkDto => new Link { Title = linkDto.Title!, Url = linkDto.Url!, Id = linkDto.Id }).ToList(),
            Contributors = transformed.Contributors.Select(contributorDto => new Contributor { UserId = contributorDto.UserId, WikiId = contributorDto.WikiId, ContributorRoleId = contributorDto.ContributorRoleId }).ToList()
        };
    }

    public void Map(Wiki source, Wiki destination)
    {
        destination.Id = source.Id;
        destination.Name = source.Name;
        destination.IsArchived = source.IsArchived;
        destination.Pages = source.Pages;
        destination.BackgroundImagePath = source.BackgroundImagePath;
        destination.MainWikiImagePath = source.MainWikiImagePath;
        destination.MainLinks = source.MainLinks;
        destination.Contributors = source.Contributors;
    }
}