using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timecop.Library.Shared;
using Timecop.Integrations.Models;
using Timecop.Integrations.Jira.Models;

namespace Timecop.Library.Components.Authentication;

public class LoginBase : ComponentBase
{
    public ElementReference EmailInputRef;

    [CascadingParameter]
    public AppState AppState { get; set; } = null!;

    //! In development
    public string JiraEmail { get; set; } = "";

    //public string JiraEmail { get; set; } = "";
    //public string JiraToken { get; set; } = "";
    public string JiraToken { get; set; } = "";

    public LoginStatus LoginStatus { get; set; }

    public async void Login()
    {
        LoginStatus = await AppState.Login(JiraEmail, JiraToken);
        StateHasChanged();
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {

        if (firstRender)
        {
            // TODO: Ask Nicholas if he knows why three dots here and what to return
            _ = EmailInputRef.FocusAsync();
            Console.WriteLine("test");
        }

        return Task.CompletedTask;
        // used in guide https://swimburger.net/blog/dotnet/how-to-run-code-after-blazor-component-has-rendered
        //return base.OnAfterRenderAsync(firstRender);
    }
}
