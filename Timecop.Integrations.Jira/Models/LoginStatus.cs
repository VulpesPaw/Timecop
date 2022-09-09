using System;
using System.Collections.Generic;
using System.Text;

namespace Timecop.Integrations.Jira.Models;

public class LoginStatus
{
    public int? ErrorCode { get; set; }
    public bool IsValid { get; set; }

    //public bool? IsValidEmail { get; set; }
    public string? Message { get; set; }
}
