using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Effectsoft.TempoSync.Application.Dto;

public class PostWorklogDto
{
    public List<WorklogAttribute> attributes { get; set; }
    public string? authorAccountId { get; set; }
    public Int64? billableSeconds { get; set; }
    public string description { get; set; }
    public string issueKey { get; set; }
    public Int64 remainingEstimateSeconds { get; set; }
    public string startDate { get; set; }
    public string startTime { get; set; }
    public Int64 timeSpentSeconds { get; set; }
}

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class WorklogAttribute
{
    public string key { get; set; }
    public string value { get; set; }
}
