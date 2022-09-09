using System;

namespace Timecop.Integrations.Tempo;

public class Worklog
{
    public string AccountKey { get; set; }
    public string AuthorFullName { get; set; }
    public string AuthorId { get; set; }
    public Double BillableSeconds { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Description { get; set; }
    public int Id { get; set; }
    public string IssueKey { get; set; }
    public Int64? JiraWorklogId { get; set; }
    public Int64 LocalTempoWorklogId { get; set; }
    public Int64 remainingEstimateSeconds { get; set; }
    public string RemoteResponseStatusCode { get; set; }
    public Int64 RemoteTempoWorklogId { get; set; }
    public string StartDate { get; set; }
    public string StartTime { get; set; }
    public Double TimeSpentSeconds { get; set; }
    public DateTime UpdatedAt { get; set; }
}
