using viki_01.Entities;

namespace viki_01.Services;

public interface IFeedbackRepository
{
    public Task<Feedback?> GetFeedbackAsync(int feedbackId);
    public Task<IEnumerable<Feedback>> GetFeedbacksAsync();
    public Task CreateFeedbackAsync(Feedback feedback);
}