using viki_01.Entities;
using viki_01.Models.Dto;

namespace viki_01.Services.Mappers;

public class PageToPageUpsertDtoMapper : IMapper<Page, PageUpsertDto>
{
    public PageUpsertDto Map(Page original)
    {
        return new PageUpsertDto
        {
            ProcessedHtml = original.ProcessedHtml,
            RawHtml = original.RawHtml
        };
    }

    public Page Map(PageUpsertDto transformed)
    {
        return new Page
        {
            RawHtml = transformed.RawHtml,
            ProcessedHtml = transformed.ProcessedHtml
        };
    }

    public void Map(Page source, Page destination)
    {
        destination.ProcessedHtml = source.ProcessedHtml;
        destination.RawHtml = source.RawHtml;
    }
}