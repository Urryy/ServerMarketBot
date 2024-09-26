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
                var newUser = new User(chatId, upd.Message?.Chat.Username ?? upd.Message?.Chat.LastName ?? "unknown user", Role.User, UserCommands.Start);
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
                user.Command = UserCommands.ChooseTeam;
                await client.SendTextMessageAsync(chatId, "Для того, чтобы сделать запрос на пополнение - нажмите на кнопку \"Пополнить\"",
                    replyMarkup: InlineButtonMessage.GetFillButtons());
            }

            if(text == BotCommands.FillCommand)
            {
                user.Command = UserCommands.TypeFill;
                await client.SendMessageAsync(upd, user, "Выберите тип пополнения", InlineButtonMessage.GetTypesFillButtons());
            }

            if(text == BotCommands.AgentsCommand)
            {
                user.Command = UserCommands.Agents;
                await client.SendMessageAsync(upd, user, "Выберите одного из агентов", InlineButtonMessage.GetTypesFillAgentsButtons());
            }

            if(text == BotCommands.PremiumAgency)
            {
                user.Command = UserCommands.PremiumAgency;
                await client.SendMessageAsync(upd, user, "Выберите группу", InlineButtonMessage.GetGroupsButtons());
            }

            if (text.IsGroupCommand())
            {
                user.Command = UserCommands.PremiumAgencyGroup;
                var applicationRepository = scope.ServiceProvider.GetRequiredService<IRepository<Application>>();
                var count = (await applicationRepository.GetAllAsync()).Count + 1;
                var group = text.GetGroup();
                var application = new Application(user.Id, TypeApplication.Agents, AgentApplication.PremiumAgency, count, string.Empty, group);
                await applicationRepository.AddAsync(application);
                await client.SendMessageAsync(upd, user, BotCommands.BotInfoServerAgentsPremiumAgencyGroupCommand);
            }

            if (text == BotCommands.Luca)
            {
                user.Command = UserCommands.Luca;
                var applicationRepository = scope.ServiceProvider.GetRequiredService<IRepository<Application>>();
                var count = (await applicationRepository.GetAllAsync()).Count + 1;
                var application = new Application(user.Id, TypeApplication.Agents, AgentApplication.Luca, count, string.Empty);
                await applicationRepository.AddAsync(application);

                await client.SendMessageAsync(upd, user, BotCommands.BotInfoServerAgentsLucaCommand);
            }

            if (text == BotCommands.QuangCao)
            {
                user.Command = UserCommands.QuangCao;
                var applicationRepository = scope.ServiceProvider.GetRequiredService<IRepository<Application>>();
                var count = (await applicationRepository.GetAllAsync()).Count + 1;
                var application = new Application(user.Id, TypeApplication.Agents, AgentApplication.QuangCao, count, string.Empty);
                await applicationRepository.AddAsync(application);

                await client.SendMessageAsync(upd, user, BotCommands.BotInfoServerAgentsQuangCaoCommand);
            }

            if(!text.IsGroupCommand() && user.Command == UserCommands.PremiumAgencyGroup)
            {
                user.Command = UserCommands.DraftApplication;
                var applicationRepository = scope.ServiceProvider.GetRequiredService<IRepository<Application>>();
                var application = (await applicationRepository.GetAllByExpressionAsync(i =>
                    i.Type == TypeApplication.Agents && i.Agent == AgentApplication.PremiumAgency))
                    .OrderByDescending(i => i.CreatedDate).FirstOrDefault();
                if(application == null) 
                {
                    await client.SendTextMessageAsync(chatId, "Упс, произошла ошибка, введите значение еще раз"); 
                    return;
                }
                application.Message = text;
                await applicationRepository.UpdateAsync(application);
                await ExecuteApplicationWithGroup(user, client, upd, application);
            }

            if(text != BotCommands.Luca && user.Command == UserCommands.Luca)
            {
                user.Command = UserCommands.DraftApplication;
                var applicationRepository = scope.ServiceProvider.GetRequiredService<IRepository<Application>>();
                var application = (await applicationRepository.GetAllByExpressionAsync(i =>
                    i.Type == TypeApplication.Agents && i.Agent == AgentApplication.Luca))
                    .OrderByDescending(i => i.CreatedDate).FirstOrDefault();
                if (application == null)
                {
                    await client.SendTextMessageAsync(chatId, "Упс, произошла ошибка, введите значение еще раз");
                    return;
                }
                application.Message = text;
                await applicationRepository.UpdateAsync(application);
                await ExecuteApplication(user, client, upd, application);
            }

            if (text != BotCommands.QuangCao && user.Command == UserCommands.QuangCao)
            {
                user.Command = UserCommands.DraftApplication;
                var applicationRepository = scope.ServiceProvider.GetRequiredService<IRepository<Application>>();
                var application = (await applicationRepository.GetAllByExpressionAsync(i =>
                    i.Type == TypeApplication.Agents && i.Agent == AgentApplication.QuangCao))
                    .OrderByDescending(i => i.CreatedDate).FirstOrDefault();
                if (application == null)
                {
                    await client.SendTextMessageAsync(chatId, "Упс, произошла ошибка, введите значение еще раз");
                    return;
                }
                application.Message = text;
                await applicationRepository.UpdateAsync(application);
                await ExecuteApplication(user, client, upd, application);
            }

            if(text.Contains(BotCommands.ApprovedByUserCommand))
            {
                var appId = text.Split("_")[1];
                var applicationRepository = scope.ServiceProvider.GetRequiredService<IRepository<Application>>();
                var application = await applicationRepository.GetByIdAsync(Guid.Parse(appId));
                application.State = State.InUnderСonsideration;
                await applicationRepository.UpdateAsync(application);

                var message = $"Ваша заявка №{application.Sequence} была отправлена ✔️\r\n" +
                    $"Ожидайте сообщение о подтверждении\r\n" +
                    $"Для того, чтобы оформить новую заявку - нажмите кнопку \"Пополнить\"\r\n";
                await client.SendMessageAsync(upd, user, message, InlineButtonMessage.GetFillButtons());
            }

            if (text.Contains(BotCommands.CancelledCommand))
            {
                var appId = text.Split("_")[1];
                var applicationRepository = scope.ServiceProvider.GetRequiredService<IRepository<Application>>();
                var application = await applicationRepository.GetByIdAsync(Guid.Parse(appId));
                application.State = State.Cancelled;
                await applicationRepository.UpdateAsync(application);

                await client.SendMessageAsync(upd, user, "Ваша заявка была отменена ❌", InlineButtonMessage.GetFillButtons());
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

    private async Task ExecuteApplication(User user, ITelegramBotClient client, Update upd, Entities.Application app)
    {
        var message = $"Заявка №{app.Sequence} на пополнение:\r\n" +
            $"Type: {app.Type.GetNameTypeApplication()}\r\n" +
            $"Agent: {app.Agent.ToString()}\r\n" +
            $"Server and sum: {app.Message}\r\n\n" +
            $"Подтвердите или отклоните заявку";

        await client.SendMessageAsync(upd, user, message, InlineButtonMessage.GetApproveAndCancelledButtonsByUser(app.Id));
    }

    private async Task ExecuteApplicationWithGroup(User user, ITelegramBotClient client, Update upd, Entities.Application app)
    {
        var message = $"Заявка №{app.Sequence} на пополнение:\r\n" +
            $"Type: {app.Type.GetNameTypeApplication()}\r\n" +
            $"Agent: {app.Agent.ToString()}\r\n" +
            $"Group: {app.Group}\r\n" +
            $"Server and sum: {app.Message}\r\n\n" +
            $"Подтвердите или отклоните заявку";

        await client.SendMessageAsync(upd, user, message, InlineButtonMessage.GetApproveAndCancelledButtonsByUser(app.Id));
    }

    private async Task ExecuteStart(ITelegramBotClient client, long chatId)
    {
        await client.SendTextMessageAsync(chatId, "Добро пожаловать. Укажите в какой команде вы находитесь и мы продолжим.",
                    replyMarkup: InlineButtonMessage.GetStartChooseTeamButtons());
    }
}
