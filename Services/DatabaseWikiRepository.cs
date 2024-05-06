using Microsoft.EntityFrameworkCore;
using viki_01.Contexts;
using viki_01.Entities;
using viki_01.Models.Dto;

namespace viki_01.Services;

public class DatabaseWikiRepository(WikiHostingSqlServerContext context, IMapper<Wiki, WikiDto> wikiMapper) : IWikiRepository
{
    public async Task<ICollection<Wiki>> GetAllAsync(string? search, string? topic)
    {
        IQueryable<Wiki> wikis = context.Wikis;
        if (!string.IsNullOrWhiteSpace(search))
        {
            wikis = wikis.Where(w => w.Name.Contains(search));
        }
        if (!string.IsNullOrWhiteSpace(topic))
        {
            wikis = wikis.Where(w => w.Topics.Any(t => t.Name.ToUpper().Equals(topic.ToUpper())));
        }

        return await wikis.ToListAsync();
    }

    public async Task<Wiki?> GetAsync(int wikiId)
    {
        var wiki = await context.Wikis
            .Include(wiki => wiki.Pages)
            .Include(wiki => wiki.MainLinks)
            .Include(wiki => wiki.Contributors)
            .ThenInclude(contributor => contributor.User)  // Include the related User entities
            .FirstOrDefaultAsync(w => w.Id == wikiId);

        return wiki;
    }

    public async Task AddAsync(Wiki wiki)
    {
        await context.AddAsync(wiki);
        await context.SaveChangesAsync();
    }

    public async Task EditAsync(int wikiId, Wiki editedWiki)
    {
        var wiki = await context.Wikis.FirstOrDefaultAsync(w => w.Id == wikiId);
        if (wiki is null) throw new InvalidOperationException($"Wiki with ID {wikiId} not found");

        wikiMapper.Map(editedWiki, wiki); 
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int wikiId)
    {
        var wiki = await context.Wikis.FirstOrDefaultAsync(w => w.Id == wikiId);
        if (wiki is null) throw new InvalidOperationException($"Wiki with ID {wikiId} not found");
        
        context.Wikis.Remove(wiki);
        await context.SaveChangesAsync();
    }
}