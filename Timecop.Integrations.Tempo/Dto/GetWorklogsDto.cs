using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Timecop.Integrations.Tempo;

public class Attributes
{
    [JsonProperty("self")]
    public string self { get; set; }

    [JsonProperty("values")]
    public IList<KeyValues> values { get; set; }
}

public class Author
{
    [JsonProperty("accountId")]
    public string accountId { get; set; }

    [JsonProperty("displayName")]
    public string displayName { get; set; }

    [JsonProperty("self")]
    public string self { get; set; }
}

public class GetWorklogsDto
{
    [JsonProperty("metadata")]
    public Metadata metadata { get; set; }

    [JsonProperty("results")]
    public IList<Result> results { get; set; }

    [JsonProperty("self")]
    public string self { get; set; }
}

public class Issue
{
    [JsonProperty("id")]
    public int id { get; set; }

    [JsonProperty("key")]
    public string key { get; set; }

    [JsonProperty("self")]
    public string self { get; set; }
}

public class KeyValues
{
    public string key { get; set; }
    public string value { get; set; }
}

public class Metadata
{
    [JsonProperty("count")]
    public int count { get; set; }

    [JsonProperty("limit")]
    public int limit { get; set; }

    [JsonProperty("next")]
    public string next { get; set; }

    [JsonProperty("offset")]
    public int offset { get; set; }
}

public class Result
{
    [JsonProperty("attributes")]
    public Attributes attributes { get; set; }

    [JsonProperty("author")]
    public Author author { get; set; }

    [JsonProperty("billableSeconds")]
    public Int64 billableSeconds { get; set; }

    [JsonProperty("createdAt")]
    public DateTime createdAt { get; set; }

    [JsonProperty("description")]
    public string description { get; set; }

    [JsonProperty("issue")]
    public Issue issue { get; set; }

    [JsonProperty("jiraWorklogId")]
    public Int64? jiraWorklogId { get; set; }

    [JsonProperty("self")]
    public string self { get; set; }

    [JsonProperty("startDate")]
    public string startDate { get; set; }

    [JsonProperty("startTime")]
    public string startTime { get; set; }

    [JsonProperty("tempoWorklogId")]
    public Int64 tempoWorklogId { get; set; }

    [JsonProperty("timeSpentSeconds")]
    public Int64 timeSpentSeconds { get; set; }

    [JsonProperty("updatedAt")]
    public DateTime updatedAt { get; set; }
}
