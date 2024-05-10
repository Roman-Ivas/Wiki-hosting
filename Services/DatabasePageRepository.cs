using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using viki_01.Contexts;
using viki_01.Entities;
using viki_01.Models.Dto;

namespace viki_01.Services;

public class DatabasePageRepository(WikiHostingSqlServerContext context, IMapper<Page, PageDto> pageMapper, IMapper<User, AuthorDto> authorMapper, IMapper<Wiki, WikiDto> wikiMapper) : IPageRepository
{
    public async Task<ICollection<Page>> GetAllAsync(int wikiId)
    {
        return await context.Pages
            .Where(page => page.WikiId == wikiId)
            .Include(page => page.UserRatings)
            .ToListAsync();
    }

    public async Task<ICollection<PageFeedDto>> GetRelevantPagesAsync(int limit = 20, int? topicId = null)
    {
        //var pages = await context.Pages
        //    .OrderByDescending(page => page.CreatedAt)
        //    .Take(limit)
        //    .Include(page => page.Author)
        //    .Include(page => page.UserRatings)
        //    .Include(page => page.Comments)
        //    .Include(page => page.Wiki).ThenInclude(wiki => wiki.Topics)
        //    .ToListAsync();

        var query = context.Pages
     .OrderByDescending(page => page.CreatedAt)
     .Include(page => page.Author)
     .Include(page => page.UserRatings)
     .Include(page => page.Comments)
     .Include(page => page.Wiki).ThenInclude(wiki => wiki.Topics) as IQueryable<Page>;



        if (topicId.HasValue)
        {
            query = query.Where(page => page.Wiki.Topics.Any(topic => topic.Id == topicId.Value));
        }

        var pages = await query
            .Take(limit)
            .ToListAsync();

        var pageFeedDtos = pages
            .Select(page =>
            {
                var document = new HtmlDocument();
                document.LoadHtml(page.ProcessedHtml);

                var titleNode =
                    document.DocumentNode.SelectSingleNode("//text()[normalize-space(.) != '']");
                var title = titleNode?.InnerText.Trim() ?? "No title";

                var imageNode = document.DocumentNode.SelectSingleNode("//img");
                var imagePath = imageNode?.GetAttributeValue("src", "No image") ?? "No image";

                return new PageFeedDto
                {
                    Id = page.Id,
                    Title = title,
                    ImagePath = imagePath,
                    CreatedAt = page.CreatedAt,
                    EditedAt = page.EditedAt,
                    WikiId = page.WikiId,
                    Wiki = wikiMapper.Map(page.Wiki),
                    Author = authorMapper.Map(page.Author),
                    NumberOfComments = page.Comments.Count,
                    NumberOfLikes = page.UserRatings.Aggregate(0,
                        (acc, rating) => acc + (rating.NumberOfLikes - rating.NumberOfDislikes), result => Math.Max(0, result)),
                     TopicId = page.Wiki.Topics.FirstOrDefault()?.Id ?? 0
                };
            });
        
        return pageFeedDtos.ToList();
    }

    public Task<Page?> GetAsync(int id)
    {
        return context.Pages.Include(page => page.UserRatings).FirstOrDefaultAsync(page => page.Id == id);
    }

    public Task<Page?> GetAsync(string wikiName, string pageName)
    {
        return context.Pages
            .Include(page => page.UserRatings)
            .Include(page => page.Wiki)
            .FirstOrDefaultAsync(page =>
                page.Wiki.Name == wikiName &&
                page.ProcessedHtml.ToLower().Contains(pageName.ToLower()));
    }

    public async Task AddAsync(Page page)
    {
        await context.AddAsync(page);
        await context.SaveChangesAsync();
    }

    public async Task EditAsync(int id, Page editedPage)
    {
        var page = await context.Pages.FirstOrDefaultAsync(page => page.Id == id);
        if (page is null)
        {
            throw new InvalidOperationException($"Page with ID {id} not found");
        }

        pageMapper.Map(editedPage, page);
        await context.SaveChangesAsync();
    }

    public Task DeleteAsync(int id)
    {
        var page = context.Pages.FirstOrDefault(page => page.Id == id);
        if (page is null)
        {
            throw new InvalidOperationException($"Page with ID {id} not found");
        }
        
        context.Remove(page);
        return context.SaveChangesAsync();
    }
}