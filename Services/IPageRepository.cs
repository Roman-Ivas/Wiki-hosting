using viki_01.Entities;
using viki_01.Models.Dto;

namespace viki_01.Services;

public interface IPageRepository
{
    public Task<ICollection<Page>> GetAllAsync(int wikiId);
    public Task<ICollection<PageFeedDto>> GetRelevantPagesAsync(int limit = 20);
    public Task<Page?> GetAsync(int id);
    public Task<Page?> GetAsync(string wikiName, string pageName);
    public Task AddAsync(Page page);
    public Task EditAsync(int id, Page editedPage);
    public Task DeleteAsync(int id);
}