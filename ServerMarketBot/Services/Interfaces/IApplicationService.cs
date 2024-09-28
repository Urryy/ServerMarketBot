using ServerMarketBot.Entities;
using ServerMarketBot.Entities.Common;

namespace ServerMarketBot.Services.Interfaces;

public interface IApplicationService
{
    Task<Application> UpdateState(IServiceScope scope, State state, string id, string moderationMessage = null);
}
