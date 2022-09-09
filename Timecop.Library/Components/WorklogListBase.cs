using Effectsoft.TempoSync.Application.Dto;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timecop.Integrations;
using Timecop.Integrations.Tempo;
using Timecop.Library.Shared;

namespace Timecop.Library.Components;

public partial class WorklogListBase : ComponentBase
{
    [CascadingParameter]
    public AppState AppState { get; set; }

    public List<Worklog> WorklogList
    {
        get => AppState.WorklogList;
        set => AppState.WorklogList = value;
    }

    [Inject]
    private IJiraService _JiraService { get; set; }

    [Inject]
    private ITempoService _TempoService { get; set; }

    public async Task<PostWorklogResponseDto> UpdateWorklogDescription(Worklog worklog, string description)
    {
        PostWorklogDto newWorklog = new PostWorklogDto()
        {
            issueKey = worklog.IssueKey,
            timeSpentSeconds = (long)worklog.TimeSpentSeconds,
            startDate = worklog.StartDate,
            startTime = worklog.StartTime,
            description = description,
            authorAccountId = worklog.AuthorId,
            remainingEstimateSeconds = worklog.remainingEstimateSeconds,
            billableSeconds = (long)worklog.BillableSeconds
        };
        var worklogCallback = await _TempoService.UpdateWorklog(worklog.Id, newWorklog);
        return worklogCallback;
    }

    public async Task<PostWorklogResponseDto> UpdateWorkogTime(Worklog worklog, long timeSpendSeconds)
    {
        PostWorklogDto newWorklog = new PostWorklogDto()
        {
            issueKey = worklog.IssueKey,
            timeSpentSeconds = timeSpendSeconds,
            startDate = worklog.StartDate,
            startTime = worklog.StartTime,
            description = worklog.Description,
            authorAccountId = worklog.AuthorId,
            remainingEstimateSeconds = worklog.remainingEstimateSeconds,
            billableSeconds = (long)worklog.BillableSeconds
        };
        var worklogCallback = await _TempoService.UpdateWorklog(worklog.Id, newWorklog);
        return worklogCallback;
    }

    protected override async Task OnInitializedAsync()
    {
        AppState.LoadWorklogList();
    }
}
