using Atlassian.Jira;
using System.Collections.Generic;
using System.Threading.Tasks;
using Timecop.Integrations.Jira.Models;
using Timecop.Integrations.Models;

namespace Timecop.Integrations;

public interface IJiraService
{
    Task<Issue> GetIssueByIssueKey(string issueKey);

    Task<IEnumerable<Issue>> GetJiraIssuesForProject(string projectKey);

    Task<IEnumerable<Project>> GetJiraProjects();

    Task<string> GetMyAccountId();

    Task<IEnumerable<Issue>> GetMyJiraIssues();

    Task<JiraUser> GetMySelf();

    Task<LoginStatus> SetJiraRestClient(JiraAuthentication jiraAuthentication);

    Task<LoginStatus> VerifyLogin();
}
