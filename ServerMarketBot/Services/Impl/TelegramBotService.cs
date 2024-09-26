using ServerMarketBot.Buttons;
using ServerMarketBot.Commands;
using ServerMarketBot.Entities;
using ServerMarketBot.Entities.Common;
using ServerMarketBot.Extension;
using ServerMarketBot.Repository.Interfaces;
using ServerMarketBot.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ServerMarketBot.Services.Impl;

using User = Entities.User;

public class TelegramBotService : ITelegramBotService
{
    private readonly IServiceProvider _provider;
    public TelegramBotService(IServiceProvider provider)
    {
        _provider = provider;
    }
    public async Task StartAsync(ITelegramBotClient client, Update upd)
    {
        var chatId = await upd.GetChatId();

        try
        {
            if (chatId != null)
            {
                await ExecuteCommands(client, upd, chatId.Value);
            }
            else
            {
                throw new Exception();
            }
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    private async Task ExecuteCommands(ITelegramBotClient client, Update upd, long chatId)
    {
        using(var scope = _provider.CreateScope())
        {
            var text = await upd.GetText();
            if (string.IsNullOrEmpty(text)) return;

            var userRepository = scope.ServiceProvider.GetRequiredService<IRepository<User>>();
            var user = await userRepository.GetByExpressionAsync(i => i.ChatTelegramId == chatId);

            if(user == null)
            {
                var newUser = new User(chatId, upd.Message?.Chat.Username ?? upd.Message?.Chat.LastName ?? "unknown user", Entities.Common.Role.User);
                await userRepository.AddAsync(newUser);
                await ExecuteStart(client, chatId);
                return;
            }

            if(text == "/start")
            {
                await ExecuteStart(client, chatId);
                return;
            }

            if(text.IsTeamCommand())
            {
                user.Team = text.Replace("Command", "").ToEnum<Team>();
                await client.SendTextMessageAsync(chatId, "Для того, чтобы сделать запрос на пополнение - нажмите на кнопку \"Пополнить\"",
                    replyMarkup: InlineButtonMessage.GetFillButtons());
            }

            if(text == BotCommands.FillCommand)
            {
                await client.SendMessageAsync(upd, user, "Выберите тип пополнения", InlineButtonMessage.GetTypesFillButtons());
            }

            if(text == BotCommands.AgentsCommand)
            {
                await client.SendMessageAsync(upd, user, "Выберите одного из агентов", InlineButtonMessage.GetTypesFillAgentsButtons());
            }

            if(text == BotCommands.PremiumAgency)
            {
                await client.SendMessageAsync(upd, user, "Выберите группу", InlineButtonMessage.GetGroupsButtons());
            }

            if (text.IsGroupCommand())
            {
                var applicationRepository = scope.ServiceProvider.GetRequiredService<IRepository<Application>>();
                var count = (await applicationRepository.GetAllAsync()).Count + 1;
                var group = text.GetGroup();
                var application = new Application(user.Id, TypeApplication.Agents, AgentApplication.PremiumAgency, count, string.Empty, group);
                await applicationRepository.AddAsync(application);
                await client.SendMessageAsync(upd, user, BotCommands.BotInfoServerAgentsPremiumAgencyGroupCommand);
            }

            if (text == BotCommands.Luca)
            {
                var applicationRepository = scope.ServiceProvider.GetRequiredService<IRepository<Application>>();
                var count = (await applicationRepository.GetAllAsync()).Count + 1;
                var application = new Application(user.Id, TypeApplication.Agents, AgentApplication.Luca, count, string.Empty);
                await applicationRepository.AddAsync(application);

                await client.SendMessageAsync(upd, user, BotCommands.BotInfoServerAgentsLucaCommand);
            }

            if (text == BotCommands.QuangCao)
            {
                var applicationRepository = scope.ServiceProvider.GetRequiredService<IRepository<Application>>();
                var count = (await applicationRepository.GetAllAsync()).Count + 1;
                var application = new Application(user.Id, TypeApplication.Agents, AgentApplication.QuangCao, count, string.Empty);
                await applicationRepository.AddAsync(application);

                await client.SendMessageAsync(upd, user, BotCommands.BotInfoServerAgentsQuangCaoCommand);
            }

            if (text == BotCommands.LamanshCommand)
            {

            }

            if(text == BotCommands.RashodnikiCommand)
            {

            }

            await userRepository.UpdateAsync(user);
        }
    }

    private async Task ExecuteStart(ITelegramBotClient client, long chatId)
    {
        await client.SendTextMessageAsync(chatId, "Добро пожаловать. Укажите в какой команде вы находитесь и мы продолжим.",
                    replyMarkup: InlineButtonMessage.GetStartChooseTeamButtons());
    }
}
