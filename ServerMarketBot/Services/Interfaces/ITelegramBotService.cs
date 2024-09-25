using Telegram.Bot;
using Telegram.Bot.Types;

namespace ServerMarketBot.Services.Interfaces;

public interface ITelegramBotService
{
    Task StartAsync(ITelegramBotClient client, Update upd);
}
