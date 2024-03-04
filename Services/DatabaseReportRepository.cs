using Microsoft.EntityFrameworkCore;
using viki_01.Contexts;
using viki_01.Entities;

namespace viki_01.Services;

public class DatabaseReportRepository(WikiHostingSqlServerContext context) : IReportRepository
{
    public async Task<Report?> GetAsync(int id)
    {
        return await context.Reports.FirstOrDefaultAsync(x => x.Id == id);
    }
    
    public async Task<ICollection<Report>> GetAllAsync(int? userId = null, string? contentUrl = null)
    {
        IQueryable<Report> reports = context.Reports;

        if (userId is not null)
        {
            reports = reports.Where(x => x.UserId == userId);
        }
        
        if (contentUrl is not null)
        {
            reports = reports.Where(x => x.ReportedContentUrl == contentUrl);
        }

        return await reports.ToListAsync();
    }

    public async Task CreateAsync(Report report)
    {
        await context.Reports.AddAsync(report);
        await context.SaveChangesAsync();
    }
}