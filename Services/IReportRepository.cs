using viki_01.Entities;

namespace viki_01.Services;

public interface IReportRepository
{
    public Task<Report?> GetAsync(int id);
    public Task<ICollection<Report>> GetAllAsync(int? userId = null, string? contentUrl = null);
    public Task CreateAsync(Report report);
}