using Microsoft.EntityFrameworkCore;
using viki_01.Contexts;
using viki_01.Entities;
using viki_01.Models.Dto;

namespace viki_01.Services;

public class DatabaseTopicRepository(WikiHostingSqlServerContext context, IMapper<Topic, TopicDto> topicMapper)
    : ITopicRepository
{
    public async Task<ICollection<Topic>> GetAllAsync(string? search = null)
    {
        IQueryable<Topic> query = context.Topics;

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(topic => topic.Name.Contains(search));
        }

        return await query.ToListAsync();
    }

    public async Task<Topic?> GetAsync(int id)
    {
        var topic = await context.Topics.FirstOrDefaultAsync(topic => topic.Id == id);
        return topic;
    }
    
    public async Task AddAsync(Topic topic)
    {
        await context.Topics.AddAsync(topic);
        await context.SaveChangesAsync();
    }

    public async Task EditAsync(int id, Topic editedTopic)
    {
        var topic = await context.Topics.FirstOrDefaultAsync(topic => topic.Id == id);
        if (topic is null)
        {
            throw new InvalidOperationException($"Topic with ID {id} not found");
        }

        topicMapper.Map(editedTopic, topic);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var topic = await context.Topics.FirstOrDefaultAsync(topic => topic.Id == id);
        if (topic is null)
        {
            throw new InvalidOperationException($"Topic with ID {id} not found");
        }

        context.Remove(topic);
        await context.SaveChangesAsync();
    }
}