using viki_01.Dto;
using viki_01.Entities;

namespace viki_01.Services;

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