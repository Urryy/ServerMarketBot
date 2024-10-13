using ServerMarketBot.Commands;
using ServerMarketBot.Entities;
using ServerMarketBot.Entities.Common;
using ServerMarketBot.Extension;
using ServerMarketBot.Repository.Interfaces;
using ServerMarketBot.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = ServerMarketBot.Entities.User;

namespace ServerMarketBot.Services.Impl;

public class UserService : IUserService
{
    private readonly IServiceProvider _provider;
    public UserService(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task<Entities.User?> GetOrCreateUser(ITelegramBotClient client, Update upd, long chatId)
    {
        using (var scope = _provider.CreateScope())
        {
            var userRepository = scope.ServiceProvider.GetRequiredService<IRepository<User>>();
            var telegramId = upd.GetTelegramId();
            var user = await userRepository.GetByExpressionAsync(i => i.TelegramId == telegramId);

            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var isMember = await client.IsMemberOfChannel(upd, configuration);

            if (!isMember) return null;

            if (configuration["ChatMembersId"] == chatId.ToString()) return user;

            if (user == null)
            {
                var tId = upd.GetTelegramId();
                var newUser = new User(tId.Value, chatId, upd.GetUserName(), Role.User, UserCommands.Start);
                await userRepository.AddAsync(newUser);
                return newUser;
            }
            else
            {
                return user;
            }
        }
    }
}
