using viki_01.Entities;

namespace viki_01.Services;

public interface ITopicRepository
{
    public Task<ICollection<Topic>> GetAllAsync(string? search = null);
    public Task<Topic?> GetAsync(int id);
    public Task AddAsync(Topic topic);
    public Task EditAsync(int id, Topic editedTopic);
    public Task DeleteAsync(int id);
}