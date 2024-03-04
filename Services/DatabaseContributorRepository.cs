using Microsoft.EntityFrameworkCore;
using viki_01.Contexts;
using viki_01.Entities;

namespace viki_01.Services;

public class DatabaseContributorRepository(WikiHostingSqlServerContext context) : IContributorRepository
{
    public async Task<ICollection<Contributor>> GetAllAsync()
    {
        var contributors = await context.Contributors.ToListAsync();
        return contributors;
    }

    public async Task<ICollection<Contributor>> GetWikiContributors(int wikiId)
    {
        var contributors = context.Contributors.Where(contributor => contributor.WikiId == wikiId);
        return await contributors.ToListAsync();
    }

    public Task<Contributor?> GetContributor(int wikiId, int userId)
    {
        var contributor = context.Contributors.Where(contributor => contributor.WikiId == wikiId && contributor.UserId == userId).Include(contributor => contributor.ContributorRole);
        return contributor.FirstOrDefaultAsync();
    }

    public async Task AddAsync(Contributor contributor)
    {
        await context.Contributors.AddAsync(contributor);
        await context.SaveChangesAsync();
    }

    public async Task EditAsync(int id, Contributor editedContributor)
    {
        var contributor = await context.Contributors.FindAsync(id);
        
        if (contributor is null)
        {
            throw new InvalidOperationException($"Contributor with ID {id} not found");
        }

        contributor.ContributorRoleId = editedContributor.ContributorRoleId;
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var contributor = await context.Contributors.FindAsync(id);
        
        if (contributor is null)
        {
            throw new InvalidOperationException($"Contributor with ID {id} not found");
        }
        
        context.Contributors.Remove(contributor);
        await context.SaveChangesAsync();
    }
}