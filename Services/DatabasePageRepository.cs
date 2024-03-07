using Microsoft.EntityFrameworkCore;
using viki_01.Contexts;
using viki_01.Entities;
using viki_01.Models.Dto;

namespace viki_01.Services;

public class DatabasePageRepository(WikiHostingSqlServerContext context, IMapper<Page, PageDto> pageMapper) : IPageRepository
{
    public async Task<ICollection<Page>> GetAllAsync(int wikiId)
    {
        return await context.Pages
            .Where(page => page.WikiId == wikiId)
            .ToListAsync();
    }
    
    public Task<Page?> GetAsync(int id)
    {
        return context.Pages.FirstOrDefaultAsync(page => page.Id == id);
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