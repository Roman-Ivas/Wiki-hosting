using viki_01.Entities;

namespace viki_01.Services;

public interface IContributorRepository
{
    public Task<ICollection<Contributor>> GetAllAsync();
    public Task<ICollection<Contributor>> GetWikiContributors(int wikiId, string? role = null);
    public Task<Contributor?> GetContributor(int wikiId, int userId);
    public Task AddAsync(Contributor contributor);
    public Task EditAsync(int id, Contributor editedContributor);
    public Task DeleteAsync(int id);
}