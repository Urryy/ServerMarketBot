using ServerMarketBot.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ServerMarketBot.Services.Impl;

public class TelegramBotService : ITelegramBotService
{
    private readonly IServiceProvider _provider;
    public TelegramBotService(IServiceProvider provider)
    {
        _provider = provider;
    }
    public Task StartAsync(ITelegramBotClient client, Update upd)
    {
        throw new NotImplementedException();
    }
}
