using viki_01.Entities;

namespace viki_01.Services;

public interface IWikiRepository
{
    public Task<ICollection<Wiki>> GetAllAsync(string? search, string? topic);
    public Task<Wiki?> GetAsync(int wikiId);
    public Task<Wiki?> GetAsync(string wikiTitle);
    public Task AddAsync(Wiki wiki);
    public Task EditAsync(int wikiId, Wiki editedWiki);
    public Task DeleteAsync(int wikiId);
}