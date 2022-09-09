using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Timecop.Integrations;
using Timecop.Integrations.Tempo;
using Timecop.Library.Shared;
using Effectsoft.TempoSync.Application.Dto;

namespace Timecop.Library.Components.CRUD
{
    // by having a razor.cs class it's more intigrated and closer to CrudWorklog.razor
    // Partial class basicly says that this is a 'shared' class
    public partial class CrudWorklog
    {
        // Todo: Convert TimeSpentSecondsto Hours and minutes or only minutes

        // CascadingAppComponent – Gives access to CrudState
        [CascadingParameter]
        public CrudState CrudFormState { get; set; }

        [CascadingParameter]
        private AppState AppState { get; set; } = null!;

        // Allows us to refrence an element (?)
        public ElementReference TimeInputRefrence { get; set; }

        // Private variables

        [Inject]
        private IJiraService _JiraService { get; set; }

        [Inject]
        private ITempoService _TempoService { get; set; }

        public async Task<bool> SubmitWorklog()
        {
            // TODO: What will be done
            /*
             * Get Worklog for the date that will be added (often current date)
             * look throught those for an equal in issueKey, and desciripton
             * if aboce true - update exisitng, else create a new worklog
             *
             * Make use of billible and ETA seconds
             */
            //! WorklogList does not update on submit as we as of yet don't call WorkLogListBase for them
            //! to re-read the list

            // DateString used to search throug less entries
            var worklogList = await _TempoService.GetWorklogsForCurrentUser(CrudFormState.DateString, CrudFormState.DateString);
            foreach (var worklog in worklogList)
            {
                if (
                   worklog.IssueKey == this.CrudFormState.IssueKey &&
                   worklog.Description == this.CrudFormState.Description &&
                   worklog.StartDate == CrudFormState.DateString)
                {
                    await UpdateExistingWorklog(worklog);
                    StateHasChanged();

                    return true;
                }
            }

            PostWorklogDto newWorklog = new PostWorklogDto()
            {
                issueKey = this.CrudFormState.IssueKey,
                timeSpentSeconds = this.CrudFormState.TimeSpentSeconds,
                startDate = this.CrudFormState.DateString,
                startTime = "00:00:00",
                description = this.CrudFormState.Description,
                // use accunt Id from JiraUser from ...?
                authorAccountId = await _JiraService.GetMyAccountId(),
                // authorAccountId = "SERIALIZED_AUTHOR_ACCIUNT_ID",
                remainingEstimateSeconds = 0,
                billableSeconds = 0
            };

            CrudFormState.IssueKey = "";
            CrudFormState.Description = "";
            CrudFormState.TimeSpentSeconds = 0;

            await CreateNewWorklog(newWorklog);

            StateHasChanged();
            return true;
        }

        public async Task<PostWorklogResponseDto> CreateNewWorklog(PostWorklogDto worklog)
        {
            try
            {
                var worklog_callback = await _TempoService.CreateWorklog(worklog);

                AppState.LoadWorklogList();
                return worklog_callback;
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return null;
            }
        }

        public async Task<PostWorklogResponseDto> UpdateExistingWorklog(Worklog worklog)
        {
            worklog.TimeSpentSeconds += this.CrudFormState.TimeSpentSeconds;
            var worklogResult = await UpdateWorklogTime(worklog, (long)worklog.TimeSpentSeconds);
            return worklogResult;
        }

        public async Task<PostWorklogResponseDto> UpdateWorklogTime(Worklog worklog, long time_spent_seconds)
        {
            PostWorklogDto newWorklog = new PostWorklogDto()
            {
                issueKey = worklog.IssueKey,
                timeSpentSeconds = time_spent_seconds,
                startDate = worklog.StartDate,
                startTime = worklog.StartTime,
                description = worklog.Description,
                authorAccountId = worklog.AuthorId,
                remainingEstimateSeconds = worklog.remainingEstimateSeconds,
                billableSeconds = (long)worklog.BillableSeconds
            };
            var worklogResult = await _TempoService.UpdateWorklog(worklog.Id, newWorklog);
            return worklogResult;
        }
    }
}
