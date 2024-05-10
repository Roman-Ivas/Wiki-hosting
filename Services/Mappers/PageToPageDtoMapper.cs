using viki_01.Entities;
using viki_01.Models.Dto;

namespace viki_01.Services.Mappers;

// ReSharper disable once ClassNeverInstantiated.Global
public class PageToPageDtoMapper : IMapper<Page, PageDto>
{
    public PageDto Map(Page original)
    {
        return new PageDto
        {
            Id = original.Id,
            AuthorId = original.AuthorId,
            WikiId = original.WikiId,
            RawHtml = original.RawHtml,
            ProcessedHtml = original.ProcessedHtml,
            UserRatings = original.UserRatings
        };
    }

    public Page Map(PageDto transformed)
    {
        return new Page
        {
            Id = transformed.Id,
            AuthorId = transformed.AuthorId,
            WikiId = transformed.WikiId,
            RawHtml = transformed.RawHtml,
            ProcessedHtml = transformed.ProcessedHtml,
            UserRatings = transformed.UserRatings.ToList()
        };
    }

    public void Map(Page source, Page destination)
    {
        destination.Id = source.Id;
        destination.AuthorId = source.AuthorId;
        destination.WikiId = source.WikiId;
        destination.RawHtml = source.RawHtml;
        destination.ProcessedHtml = source.ProcessedHtml;
        destination.UserRatings = source.UserRatings;
    }
}    