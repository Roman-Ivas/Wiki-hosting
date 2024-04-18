using Microsoft.EntityFrameworkCore;
using viki_01.Contexts;
using viki_01.Entities;

namespace viki_01.Services;

public class DatabaseFeedbackRepository(WikiHostingSqlServerContext context) : IFeedbackRepository
{
    public async Task<Feedback?> GetFeedbackAsync(int feedbackId)
    {
        var feedback = await context.Feedbacks.FindAsync(feedbackId);
        return feedback;
    }

    public async Task<IEnumerable<Feedback>> GetFeedbacksAsync()
    {
        var feedbacks = await context.Feedbacks.ToListAsync();
        return feedbacks;
    }

    public async Task CreateFeedbackAsync(Feedback feedback)
    {
        await context.Feedbacks.AddAsync(feedback);
        await context.SaveChangesAsync();
    }
}