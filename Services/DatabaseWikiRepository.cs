using Microsoft.EntityFrameworkCore;
using viki_01.Contexts;
using viki_01.Dto;
using viki_01.Entities;

namespace viki_01.Services;

public class DatabaseWikiRepository(WikiHostingSqlServerContext context, IMapper<Wiki, WikiDto> wikiMapper) : IWikiRepository
{
    public async Task<ICollection<Wiki>> GetAllAsync(string? search)
    {
        IQueryable<Wiki> wikis = context.Wikis;
        if (!string.IsNullOrWhiteSpace(search))
        {
            wikis = wikis.Where(w => w.Name.Contains(search));
        }

        return await wikis.ToListAsync();
    }

    public async Task<Wiki?> GetAsync(int wikiId)
    {
        var wiki = await context.Wikis.FirstOrDefaultAsync(w => w.Id == wikiId);
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