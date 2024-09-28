using ServerMarketBot.Entities;
using ServerMarketBot.Entities.Common;
using ServerMarketBot.Repository.Interfaces;
using ServerMarketBot.Services.Interfaces;

namespace ServerMarketBot.Services.Impl;

public class ApplicationService : IApplicationService
{
    public async Task<Application> UpdateState(IServiceScope scope, State state, string id, string moderationMessage = null)
    {
        var applicationRepository = scope.ServiceProvider.GetRequiredService<IRepository<Application>>();
        var application = await applicationRepository.GetByIdAsync(Guid.Parse(id));
        application.State = state;
        if(moderationMessage!= null) application.ModerationMessage = moderationMessage;
        await applicationRepository.UpdateAsync(application);

        return application;
    }
}
