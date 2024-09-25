using ServerMarketBot.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ServerMarketBot;

public class TelegramBot
{
    public TelegramBotClient TelegramBotClient { get; set; }
    private readonly IConfiguration Configuration;
    private readonly ITelegramBotService _srvcBot;
    public TelegramBot(IConfiguration configuration, ITelegramBotService srvcBot)
    {
        Configuration = configuration;
        _srvcBot = srvcBot;
    }

    public async Task UpdateHandler(ITelegramBotClient client, Update upd, CancellationToken token)
    {
        try
        {
            if(TelegramBotClient.Timeout.Minutes > 1) 
            {
                await CloseAndStartConnection();
            }
            await _srvcBot.StartAsync(client, upd);
        }
        catch (Exception ex)
        {
            throw new ArgumentException(ex.Message);
        }
    }

    public async Task ExceptionHandler(ITelegramBotClient client, Exception ex, CancellationToken token)
    {
        if (TelegramBotClient.Timeout.Minutes > 1)
        {
            await CloseAndStartConnection();
        }

        await LaunchAsync();
    }

    public async Task LaunchAsync()
    {
        try
        {
            TelegramBotClient = new TelegramBotClient(Configuration["TelegramToken"]!);
            await TelegramBotClient.ReceiveAsync(UpdateHandler, ExceptionHandler);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    private async Task CloseAndStartConnection()
    {
        await TelegramBotClient.CloseAsync();
        await LaunchAsync();
    }
}
