using Atlassian.Jira;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Timecop.Integrations.Jira.Dto;
using Timecop.Integrations.Jira.Models;
using Timecop.Integrations.Models;

namespace Timecop.Integrations;

public class JiraService : IJiraService
{
    private readonly IConfiguration _config;
    private readonly ILogger<JiraService> _logger;
    private Atlassian.Jira.Jira _jira;

    public JiraService(ILogger<JiraService> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
        //_jira = Atlassian.Jira.Jira.CreateRestClient("", JiraSimpleUser.JiraEmail, JiraSimpleUser.JiraToken);
        //_jira = Atlassian.Jira.Jira.CreateRestClient("", "", "")

        //if (JiraSimpleUser.JiraEmail is not null && JiraSimpleUser.JiraToken is not null)
        //{
        //SetJiraRestClient();
        //};
    }

    public LoginStatus FailedLoginAttempt(Exception loginException)
    {
        /*

        if email is not valid for entered domain (Domain is hard coded when writing this comment)
        loginException.Message = "Response Content: {\"message\":\"Client must be authenticated to access this resource.\",\"status-code\":401}"

        if token is not correct for entered email (then the catch will return because this response is not json)
        loginException.Message = "Response Content: Basic authentication with passwords is deprecated.  For more information, see: https://confluence.atlassian.com/cloud/deprecation-of-basic-authentication-with-passwords-for-jira-and-confluence-apis-972355348.html"

         */

        try
        {
            var loginResponse = JsonSerializer.Deserialize<LoginStatusDto>(loginException.Message.Replace("Response Content: ", ""));

            return new LoginStatus()
            {
                //IsValidEmail = false,
                IsValid = false,
                Message = loginResponse?.Message,
                ErrorCode = loginResponse?.StatusCode,
            };
        }
        catch (Exception)
        {
            return new LoginStatus()
            {
                //IsValidEmail = true,
                IsValid = false,
                Message = loginException.Message,
            };
        }
    }

    public async Task<Issue> GetIssueByIssueKey(string issueKey)
    {
        _logger.LogInformation($"{DateTimeOffset.Now} - Getting issues from {issueKey} from Jira", DateTimeOffset.Now);
        return await _jira.Issues.GetIssueAsync(issueKey);
    }

    public async Task<IEnumerable<Issue>> GetJiraIssuesForProject(string projectKey)
    {
        List<Issue> issueList = new List<Issue>();
        IPagedQueryResult<Issue> issues;

        _logger.LogInformation($"{DateTimeOffset.Now} - Getting issues from Jira project {projectKey}", DateTimeOffset.Now);

        var limit = 100;
        var offset = 0;
        var count = 1;

        var jqlQuery = $"project=\"{projectKey}\"";

        while (count > 0)
        {
            _logger.LogInformation($"{DateTimeOffset.Now} - Fetching Issues from Jira for project: {projectKey}", DateTimeOffset.Now);
            _logger.LogInformation($"{DateTimeOffset.Now} - Count =  {count}", DateTimeOffset.Now);

            issues = await _jira.Issues.GetIssuesFromJqlAsync(jqlQuery, limit, offset);

            issueList.AddRange(issues);

            offset += issues.Count();
            count = issues.Count();
        }

        return issueList;
    }

    public async Task<IEnumerable<Project>> GetJiraProjects()
    {
        _logger.LogInformation($"{DateTimeOffset.Now} - Getting projectlist from Jira", DateTimeOffset.Now);

        return await _jira.Projects.GetProjectsAsync();
    }

    public async Task<string> GetMyAccountId()
    {
        var myself = await _jira.Users.GetMyselfAsync();
        return myself.AccountId;
        // Gets Account ID, use for everything
    }

    public async Task<IEnumerable<Issue>> GetMyJiraIssues()
    {
        var settings = new JiraRestClientSettings()
        {
            EnableRequestTrace = true
        };

        List<Issue> issueList = new List<Issue>();
        IPagedQueryResult<Issue> issues;

        _logger.LogInformation($"{DateTimeOffset.Now} - Getting issues from Jira user", DateTimeOffset.Now);

        var limit = 100;
        var offset = 0;
        var count = 1;

        var jqlQuery = $"assignee in (currentUser())";

        while (count > 0)
        {
            _logger.LogInformation($"{DateTimeOffset.Now} - Fetching Issues from Jira for user", DateTimeOffset.Now);
            _logger.LogInformation($"{DateTimeOffset.Now} - Count =  {count}", DateTimeOffset.Now);

            issues = await _jira.Issues.GetIssuesFromJqlAsync(jqlQuery, limit, offset);

            issueList.AddRange(issues);

            offset += issues.Count();
            count = issues.Count();
        }

        return issueList;
    }

    public async Task<JiraUser> GetMySelf()
    {
        return await _jira.Users.GetMyselfAsync();
        // Gets Account ID, use for everything
    }

    public async Task<LoginStatus> SetJiraRestClient(JiraAuthentication jiraAuthentication)
    {
        _jira = Atlassian.Jira.Jira.CreateRestClient(jiraAuthentication.Domain, jiraAuthentication.Email, jiraAuthentication.Token);

        var login = await VerifyLogin();

        return login;
    }

    public async Task<LoginStatus> VerifyLogin()
    {
        try
        {
            await GetMySelf();

            return new LoginStatus()
            {
                IsValid = true,
                //IsValidEmail = true,
            };
        }
        catch (System.Security.Authentication.AuthenticationException loginException)
        {
            return FailedLoginAttempt(loginException);
        }
        catch (Exception loginException)
        {
            return FailedLoginAttempt(loginException);
        }
    }
}
