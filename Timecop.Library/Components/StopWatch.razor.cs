using Effectsoft.TempoSync.Application.Dto;
using Microsoft.AspNetCore.Components;
using System.Diagnostics;
using System.Timers;
using Timecop.Integrations;
using Timecop.Integrations.Tempo;
using Timecop.Library.Components.CRUD;
using Timecop.Library.Shared;

namespace Timecop.Library.Components
{
    public partial class StopWatch
    {
        // Access to Crud CascadingParameter
        [CascadingParameter]
        public CrudState CrudFormState { get; set; }

        [CascadingParameter]
        private AppState AppState { get; set; } = null!;

        private Stopwatch stopwatch = new Stopwatch();
        private System.Timers.Timer timer = new System.Timers.Timer();

        [Inject]
        private IJiraService _JiraService { get; set; }

        [Inject]
        private ITempoService _TempoService { get; set; }

        private async void runWatch()
        {
            if (!stopwatch.IsRunning)
            {
                // subscribes timerEvent to timer
                timer.Elapsed += timerEvent;
                timer.Interval = 250;

                stopwatch.Start();
                // OBS! Timer runs in its own thread!
                timer.Start();
            }
            else
            {
                stopwatch.Stop();
                timer.Stop();
                //unsubscribes timerevent
                timer.Elapsed -= timerEvent;

                if (checkWorklog())
                {
                    CrudWorklog worklogHandler = new CrudWorklog();
                    await submitWorklog();
                    CrudFormState.IssueKey = String.Empty;
                    CrudFormState.Description = String.Empty;
                    CrudFormState.TimeSpentSeconds = 0;
                    CrudFormState.Date = DateTime.Now;
                    stopwatch.Reset();
                }
            }
        }

        private void timerEvent(object? sender, ElapsedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(stopwatch.Elapsed);
            // Thread-safefly updates TimeSpentSeconds
            this.InvokeAsync(delegate
            {
                CrudFormState.TimeSpentSeconds = (long)stopwatch.Elapsed.TotalSeconds;
            });
        }

        private bool checkWorklog()
        {
            if (CrudFormState.TimeSpentSeconds < 60)
            {
                // TODO: Notify and focus for User

                CrudFormState.TimeSpentSeconds = 60;

                return true;
            }
            else if (string.IsNullOrWhiteSpace(CrudFormState.IssueKey))
            {
                // TODO: Notify and focus for User
                return false;
            }
            else if (string.IsNullOrWhiteSpace(CrudFormState.Description))
            {
                // TODO: Notify and focus for User
                return false;
            }
            // Want both datetime to be date only
            else if (DateTime.ParseExact(CrudFormState.Date.ToString("yyyy-MM-dd"), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture) > DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd"), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture))
            {
                // Notify and focus for User
                // Prompt if user mean today
                return false;
            }
            else
            {
                return true;
            }
        }

        // NOTE: all code below is copies from CrudWorklog.razor.cs
        // This is done because for whatever reason CascadingAppComponent
        // CrudFormState does not work correctly accross components in this instance

        private async Task<bool> submitWorklog()
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
                    await updateExistingWorklog(worklog);
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
                // authorAccountId = "SERIALIZED_AUTHOR_ACCOUNT_ID",
                remainingEstimateSeconds = 0,
                billableSeconds = 0
            };

            CrudFormState.IssueKey = "";
            CrudFormState.Description = "";
            CrudFormState.TimeSpentSeconds = 0;

            await createNewWorklog(newWorklog);

            StateHasChanged();
            return true;
        }

        private async Task<PostWorklogResponseDto> createNewWorklog(PostWorklogDto worklog)
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

        private async Task<PostWorklogResponseDto> updateExistingWorklog(Worklog worklog)
        {
            worklog.TimeSpentSeconds += this.CrudFormState.TimeSpentSeconds;
            var worklogCallback = await updateWorklogTime(worklog, (long)worklog.TimeSpentSeconds);
            return worklogCallback;
        }

        private async Task<PostWorklogResponseDto> updateWorklogTime(Worklog worklog, long time_spent_seconds)
        {
            PostWorklogDto new_worklog = new PostWorklogDto()
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
            var worklog_callback = await _TempoService.UpdateWorklog(worklog.Id, new_worklog);
            return worklog_callback;
        }
    }
}
