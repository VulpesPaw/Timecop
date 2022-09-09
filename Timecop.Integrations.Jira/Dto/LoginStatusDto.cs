using System.Text.Json.Serialization;

namespace Timecop.Integrations.Jira.Dto
{
    public class LoginStatusDto
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("status-code")]
        public int StatusCode { get; set; }
    }
}
