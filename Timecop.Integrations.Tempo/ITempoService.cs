using Effectsoft.TempoSync.Application.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Timecop.Integrations.Tempo;

public interface ITempoService
{
    Task<PostWorklogResponseDto> CreateWorklog(PostWorklogDto postWorklogRequest);

    Task<List<Worklog>> GetWorklogsForCurrentUser();

    Task<List<Worklog>> GetWorklogsForCurrentUser(string date_start, string date_end);

    Task<PostWorklogResponseDto> UpdateWorklog(int worklogId, PostWorklogDto postWorklogRequest);
}
