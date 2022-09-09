namespace Timecop.Integrations.Models;

public class JiraAuthentication
{
    public string Domain { get; set; } = "SERIALIZED";
    public string? Email { get; set; }
    public string? Token { get; set; }

    //public JiraAuthentication(string email, string token)
    //{
    //    Email = email;
    //    Token = token;
    //}
}
