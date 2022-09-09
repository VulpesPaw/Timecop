using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Effectsoft.TempoSync.Application.Dto;

public class Attributes
{
    public string self { get; set; }
    public List<object> values { get; set; }
}

public class Author
{
    public string accountId { get; set; }
    public string displayName { get; set; }
    public string self { get; set; }
}

public class Issue
{
    public int id { get; set; }
    public string key { get; set; }
    public string self { get; set; }
}

//response when posting a worklog
public class PostWorklogResponseDto
{
    public Attributes attributes { get; set; }
    public Author author { get; set; }
    public int billableSeconds { get; set; }
    public DateTime createdAt { get; set; }
    public string description { get; set; }
    public Issue issue { get; set; }
    public int jiraWorklogId { get; set; }
    public string self { get; set; }
    public string startDate { get; set; }
    public string startTime { get; set; }
    public int tempoWorklogId { get; set; }
    public int timeSpentSeconds { get; set; }
    public DateTime updatedAt { get; set; }
}
