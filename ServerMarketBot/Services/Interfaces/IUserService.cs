using ServerMarketBot.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ServerMarketBot.Services.Interfaces;

public interface IUserService
{
    Task<Entities.User?> GetOrCreateUser(ITelegramBotClient client, Update upd, long chatId);
}
