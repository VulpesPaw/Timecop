using Effectsoft.TempoSync.Application.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Timecop.Integrations.Tempo;

public class TempoService : ITempoService
{
    private readonly IConfiguration _config;
    private readonly ILogger<TempoService> _logger;
    private const string SERIALIZEDID = "SERIALIZED_ACCESS_TOKEN";

    public TempoService(ILogger<TempoService> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }

    public async Task<PostWorklogResponseDto> CreateWorklog(PostWorklogDto postWorklogRequest)
    {
        using var tempoClient = new HttpClient();
        tempoClient.BaseAddress = new Uri($"https://api.tempo.io/core/3/");
        // API TOKEN IS A TEMPO GLOBAL TOKEN - DO NOT CHANGE
        tempoClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", SERIALIZEDID);

        //_logger.LogInformation($"{DateTimeOffset.Now} - Posting worklog to r360 for {worklog.author.displayName}, issue: {worklog.issue.key}, date: {worklog.startDate}");

        var remoteResponse = await tempoClient.PostAsJsonAsync("worklogs", postWorklogRequest);
        var remoteResponseStatus = remoteResponse.StatusCode;
        _logger.LogInformation($"{DateTimeOffset.Now} - Response: {remoteResponseStatus.ToString()}");

        var tempRemoteResponse = await remoteResponse.Content.ReadAsStringAsync();

        if (remoteResponseStatus == System.Net.HttpStatusCode.OK)
        {
            var remoteworkLog = JsonSerializer.Deserialize<PostWorklogResponseDto>(tempRemoteResponse);

            //TODO: Update local worklog with worklog ID
            //
            return remoteworkLog;
        }

        return null;
    }

    public async Task<List<Worklog>> GetWorklogsForCurrentUser()
    {
        return await GetWorklogsForCurrentUser("2021-11-08", DateTime.Now.ToString("yyyy-MM-dd"));
    }

    public async Task<List<Worklog>> GetWorklogsForCurrentUser(string date_start, string date_end)
    {
        using var tempoClient = new HttpClient();
        tempoClient.BaseAddress = new Uri($"https://api.tempo.io/core/3/");
        tempoClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", SERIALIZEDID);

        try
        {
            int limit = 1000;
            int offset = 0;

            _logger.LogInformation($"{DateTimeOffset.Now} - Getting worklogs {offset} to {offset + limit}");

            var response = await tempoClient.GetAsync($"worklogs?limit={limit}&from={date_start}&to={date_end}");
            response.EnsureSuccessStatusCode();
            var tempResponse = response.Content.ReadAsStringAsync().Result;
            var workLogs = JsonSerializer.Deserialize<GetWorklogsDto>(tempResponse);

            List<Worklog> worklogList = new List<Worklog>();

            foreach (var worklog in workLogs.results)
            {
                var accountString = worklog.attributes.values.FirstOrDefault(c => c.key == "_Konto_")?.value;
                worklogList.Add(new Worklog()
                {
                    Id = (int)worklog.tempoWorklogId,
                    IssueKey = worklog.issue.key,
                    TimeSpentSeconds = worklog.timeSpentSeconds,
                    AuthorId = worklog.author.accountId,
                    AccountKey = accountString,
                    Description = worklog.description,
                    StartDate = worklog.startDate,
                    StartTime = worklog.startTime,
                    BillableSeconds = worklog.billableSeconds,
                });
            }

            return worklogList;
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"{DateTimeOffset.Now} - Failed parsing worklogs due to {ex.Message}");
            return null;
        }
    }

    public async Task<PostWorklogResponseDto> UpdateWorklog(int worklogId, PostWorklogDto postWorklogRequest)
    {
        using var tempoClient = new HttpClient();
        tempoClient.BaseAddress = new Uri($"https://api.tempo.io/core/3/");
        // API TOKEN IS A TEMPO GLOBAL TOKEN - DO NOT CHANGE
        tempoClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", SERIALIZEDID);

        //_logger.LogInformation($"{DateTimeOffset.Now} - Posting worklog to r360 for {worklog.author.displayName}, issue: {worklog.issue.key}, date: {worklog.startDate}");

        var remoteResponse = await tempoClient.PutAsJsonAsync("worklogs/" + worklogId, postWorklogRequest);
        var remoteResponseStatus = remoteResponse.StatusCode;
        _logger.LogInformation($"{DateTimeOffset.Now} - Response: {remoteResponseStatus.ToString()}");

        var tempRemoteResponse = await remoteResponse.Content.ReadAsStringAsync();

        if (remoteResponseStatus == System.Net.HttpStatusCode.OK)
        {
            var remoteworkLog = JsonSerializer.Deserialize<PostWorklogResponseDto>(tempRemoteResponse);

            //TODO: Update local worklog with worklog ID
            //
            return remoteworkLog;
        }

        return null;
    }
}