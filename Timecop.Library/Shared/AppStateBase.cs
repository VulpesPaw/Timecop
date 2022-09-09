using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Timecop.Integrations;
using Timecop.Integrations.Jira.Models;
using Timecop.Integrations.Models;
using Timecop.Integrations.Tempo;
using Timecop.Library.Components;

namespace Timecop.Library.Shared;

public class AppStateBase : ComponentBase
{
    #region IsAuthorized

    public bool IsAuthorizationLoading { get; set; } = true;

    private bool _isAuthorized = false;

    public bool IsAuthorized
    {
        get => _isAuthorized;
        set
        {
            _isAuthorized = value;
            StateHasChanged();
        }
    }

    #endregion IsAuthorized

    #region JiraAuthentication

    private JiraAuthentication? _jiraAuthentication;

    public string? JiraEmail
    {
        get
        {
            if (_jiraAuthentication is null) return null;
            return _jiraAuthentication.Email;
        }
    }

    public string? JiraToken
    {
        get
        {
            if (_jiraAuthentication is null) return null;
            return _jiraAuthentication.Token;
        }
    }

    #endregion JiraAuthentication

    public List<Worklog> WorklogList = new List<Worklog>();

    private string endDateString = DateTime.Now.ToString("yyyy-MM-dd");

    public string EndDateString
    {
        get => endDateString;
        set
        {
            endDateString = value;
            LoadWorklogList();
        }
    }

    private string startDateString = DateTime.Now.ToString("yyyy-MM-dd");

    public string StartDateString
    {
        get { return startDateString; }
        set
        {
            startDateString = value;
            LoadWorklogList();
        }
    }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Inject]
    private IJiraService _JiraService { get; set; }

    [Inject]
    private ITempoService _TempoService { get; set; }

    [Inject]
    private ILocalStorageService _LocalStorage { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadJiraAuthenticationFromLocalStorage();
            IsAuthorizationLoading = false;
            LoadWorklogList();
        }
    }

    public async void LoadWorklogList()
    {
        //run method to get ALL worklogs for a given time -getWorklogs
        WorklogList = await _TempoService.GetWorklogsForCurrentUser(StartDateString, EndDateString);
        StateHasChanged();
    }

    public async Task<LoginStatus> Login(string jiraEmail, string jiraToken)
    {
        var jiraAuthentication = new JiraAuthentication()
        {
            Email = jiraEmail,
            Token = jiraToken
        };

        var login = await _JiraService.SetJiraRestClient(jiraAuthentication);

        if (!login.IsValid)
        {
            await Logout();
            return login;
        }

        _jiraAuthentication = jiraAuthentication;

        IsAuthorized = true;

        await SaveJiraAuthenticationToLocalStorage();

        return login;
    }

    public async Task<bool> Logout()
    {
        try
        {
            _jiraAuthentication = new JiraAuthentication();

            var successfullyRemoved = await RemoveJiraAuthenticationFromLocalStorage();

            IsAuthorized = false;

            return successfullyRemoved;
        }
        catch (Exception)
        {
            return false;
        }
    }

    #region Storage

    private async Task<bool> LoadJiraAuthenticationFromLocalStorage()
    {
        try
        {
            var jiraAuthentication = await _LocalStorage.GetItemAsync<JiraAuthentication>("jira-authentication");

            if (jiraAuthentication is null) return false;

            var login = await _JiraService.SetJiraRestClient(jiraAuthentication);

            if (!login.IsValid)
            {
                await Logout();
                return false;
            }

            _jiraAuthentication = jiraAuthentication;

            IsAuthorized = true;

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private async Task<bool> RemoveJiraAuthenticationFromLocalStorage()
    {
        try
        {
            await _LocalStorage.RemoveItemAsync("jira-authentication");

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private async Task<bool> SaveJiraAuthenticationToLocalStorage()
    {
        try
        {
            if (_jiraAuthentication is null) return false;

            await _LocalStorage.SetItemAsync<JiraAuthentication>("jira-authentication", _jiraAuthentication);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    #endregion Storage
}
