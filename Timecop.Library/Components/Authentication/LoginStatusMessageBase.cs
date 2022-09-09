using Microsoft.AspNetCore.Components;
using Timecop.Integrations.Jira.Models;

namespace Timecop.Library.Components.Authentication;

public class LoginStatusMessageBase : ComponentBase
{
    [Parameter]
    public LoginStatus LoginStatus { get; set; }
}
