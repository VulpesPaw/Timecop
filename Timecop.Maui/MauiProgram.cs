using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Timecop.Integrations;
using Timecop.Integrations.Tempo;

namespace Timecop.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .RegisterBlazorMauiWebView()
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddBlazorWebView();
        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddSingleton<IJiraService, JiraService>();
        builder.Services.AddSingleton<ITempoService, TempoService>();

        return builder.Build();
    }
}
