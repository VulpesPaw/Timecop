using Microsoft.AspNetCore.Components;
using Timecop.Library.Shared;

namespace Timecop.Library.Components;

public class DatePickerBase : ComponentBase
{
    //// TODO: Change arrows to icons

    //public string BackButton = "←";
    //public string ForwardButton = "→";

    private const string dateFormat = "yyyy-MM-dd";

    [CascadingParameter]
    private AppState AppState { get; set; } = null!;

    public DateTime EndDate
    {
        get => ParseDateString(AppState.EndDateString);
        set => AppState.EndDateString = value.ToString(dateFormat);
    }

    public DateTime StartDate
    {
        get => ParseDateString(AppState.StartDateString);
        set => AppState.StartDateString = value.ToString(dateFormat);
    }

    private static DateTime ParseDateString(string dateString)
    {
        return DateTime.ParseExact(dateString, dateFormat, System.Globalization.CultureInfo.InvariantCulture);
    }
}
