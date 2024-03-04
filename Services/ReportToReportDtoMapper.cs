using viki_01.Dto;
using viki_01.Entities;

namespace viki_01.Services;

public class ReportToReportDtoMapper : IMapper<Report, ReportUpsertDto>
{
    public ReportUpsertDto Map(Report original)
    {
        return new ReportUpsertDto
        {
            ReportedContentUrl = original.ReportedContentUrl,
            Text = original.Text,
            UserId = original.UserId
        };
    }

    public Report Map(ReportUpsertDto transformed)
    {
        return new Report
        {
            ReportedContentUrl = transformed.ReportedContentUrl,
            Text = transformed.Text,
            UserId = transformed.UserId
        };
    }

    public void Map(Report source, Report destination)
    {
        destination.ReportedContentUrl = source.ReportedContentUrl;
        destination.Text = source.Text;
        destination.UserId = source.UserId;
    }
    
}