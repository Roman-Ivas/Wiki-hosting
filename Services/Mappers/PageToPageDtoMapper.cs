using viki_01.Entities;
using viki_01.Models.Dto;

namespace viki_01.Services.Mappers;

public class PageToPageDtoMapper : IMapper<Page, PageDto>
{
    public PageDto Map(Page original)
    {
        return new PageDto
        {
            AuthorId = original.AuthorId,
            WikiId = original.WikiId,
            RawHtml = original.RawHtml,
            ProcessedHtml = original.ProcessedHtml
        };
    }

    public Page Map(PageDto transformed)
    {
        return new Page
        {
            AuthorId = transformed.AuthorId,
            WikiId = transformed.WikiId,
            RawHtml = transformed.RawHtml,
            ProcessedHtml = transformed.ProcessedHtml
        };
    }

    public void Map(Page source, Page destination)
    {
        destination.AuthorId = source.AuthorId;
        destination.WikiId = source.WikiId;
        destination.RawHtml = source.RawHtml;
        destination.ProcessedHtml = source.ProcessedHtml;
    }
}