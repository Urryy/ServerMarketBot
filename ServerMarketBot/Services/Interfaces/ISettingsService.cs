using Telegram.Bot;
using Telegram.Bot.Types;

namespace ServerMarketBot.Services.Interfaces;

public interface ISettingsService
{
    Task ExecuteSettings(ITelegramBotClient client, Update upd);

    Task SignTeam(ITelegramBotClient client, Update upd);
}
