using viki_01.Entities;

namespace viki_01.Services;

public interface IPageRepository
{
    public Task<ICollection<Page>> GetAllAsync(int wikiId);
    public Task<Page?> GetAsync(int id);
    public Task AddAsync(Page page);
    public Task EditAsync(int id, Page editedPage);
    public Task DeleteAsync(int id);
}