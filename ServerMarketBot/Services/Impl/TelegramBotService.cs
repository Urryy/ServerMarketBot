using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using ServerMarketBot.Buttons;
using ServerMarketBot.Commands;
using ServerMarketBot.Dto;
using ServerMarketBot.Entities;
using ServerMarketBot.Entities.Common;
using ServerMarketBot.Extension;
using ServerMarketBot.Repository.Interfaces;
using ServerMarketBot.Services.Interfaces;
using System.Text.RegularExpressions;
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
        var text =await upd.GetText();

        if(text == "ExceptionCommand")
        {
            throw new ArgumentException();
        }

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
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.InnerException?.Message);
            await client.SendTextMessageAsync("746672080", ex.Message +"\n\n"+ ex.InnerException?.Message);
        }
    }

    private async Task ExecuteCommands(ITelegramBotClient client, Update upd, long chatId)
    {
        using(var scope = _provider.CreateScope())
        {
            var text = await upd.GetText();
            if (string.IsNullOrEmpty(text)) return;

            var userRepository = scope.ServiceProvider.GetRequiredService<IRepository<User>>();
            var telegramId = upd.GetTelegramId();
            var user = await userRepository.GetByExpressionAsync(i => i.TelegramId == telegramId);

            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var isMember = await client.IsMemberOfChannel(upd, configuration);
            if (!isMember) return;

            if (configuration["ChatMembersId"] == chatId.ToString()) return;

            if (user == null)
            {
                var tId = upd.GetTelegramId();
                var newUser = new User(tId.Value, chatId, upd.GetUserName(), Role.User, UserCommands.Start);
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
                var messageTelegram = await client.SendTextMessageAsync(chatId, "Для того, чтобы сделать запрос на пополнение - нажмите на кнопку \"Пополнить\"",
                    replyMarkup: InlineButtonMessage.GetFillButtons());
                user.LastMessageId = messageTelegram.MessageId;
            }

            if(text == BotCommands.FillCommand)
            {
                user.Command = UserCommands.TypeFill;
                var messageTelegram = await client.SendTextMessageAsync(chatId, "Выберите тип пополнения", 
                    replyMarkup: InlineButtonMessage.GetTypesFillButtons());
                user.LastMessageId = messageTelegram.MessageId;
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
                await application.ExecuteApplicationAgentWithGroup(user, client, upd);
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
                await application.ExecuteApplicationAgent(user, client, upd);
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
                await application.ExecuteApplicationAgent(user, client, upd);
            }

            if (text == BotCommands.LamanshCommand)
            {
                user.Command = UserCommands.Lamansh;
                var appRepository = scope.ServiceProvider.GetRequiredService<IRepository<Application>>();
                var count = (await appRepository.GetAllAsync()).Count + 1;
                var application = new Application(user.Id, TypeApplication.Lamansh, AgentApplication.None, count, string.Empty);
                await appRepository.AddAsync(application);

                await client.SendMessageAsync(upd, user, BotCommands.BotInfoServerLamanshCommand);
            }

            if(text != BotCommands.LamanshCommand && user.Command == UserCommands.Lamansh)
            {
                user.Command = UserCommands.DraftApplication;
                var appRepository = scope.ServiceProvider.GetRequiredService<IRepository<Application>>();
                var application = (await appRepository.GetAllByExpressionAsync(i =>
                    i.Type == TypeApplication.Lamansh && i.Agent == AgentApplication.None))
                    .OrderByDescending(i => i.CreatedDate).FirstOrDefault();
                if (application == null)
                {
                    await client.SendTextMessageAsync(chatId, "Упс, произошла ошибка, введите значение еще раз");
                    return;
                }
                application.Message = text;
                await appRepository.UpdateAsync(application);
                await application.ExecuteApplicationLamansh(user, client, upd);
            }

            if(text == BotCommands.RashodnikiCommand)
            {
                user.Command = UserCommands.Rashodniki;

                var appRepository = scope.ServiceProvider.GetRequiredService<IRepository<Application>>();
                var count = (await appRepository.GetAllAsync()).Count + 1;
                var application = new Application(user.Id, TypeApplication.Rashodniki, AgentApplication.None, count, string.Empty);
                await appRepository.AddAsync(application);

                await client.SendMessageAsync(upd, user, BotCommands.BotInfoServerRashodnikiCommand);
            }

            if(text != BotCommands.RashodnikiCommand && user.Command == UserCommands.Rashodniki)
            {
                user.Command = UserCommands.DraftApplication;
                var appRepository = scope.ServiceProvider.GetRequiredService<IRepository<Application>>();
                var application = (await appRepository.GetAllByExpressionAsync(i =>
                    i.Type == TypeApplication.Rashodniki && i.Agent == AgentApplication.None))
                    .OrderByDescending(i => i.CreatedDate).FirstOrDefault();
                if (application == null)
                {
                    await client.SendTextMessageAsync(chatId, "Упс, произошла ошибка, введите значение еще раз");
                    return;
                }
                application.Message = text;
                await appRepository.UpdateAsync(application);
                await application.ExecuteApplicationRashodniki(user, client, upd);
            }

            if (text.Contains(BotCommands.ApprovedByUserCommand))
            {
                user.Command = UserCommands.Approved;
                var appId = text.Split("_")[1];
                var applicationRepository = scope.ServiceProvider.GetRequiredService<IRepository<Application>>();
                var application = await applicationRepository.GetByIdAsync(Guid.Parse(appId));
                application.State = State.InUnderСonsideration;
                
                var message = $"Ваша заявка №{application.Sequence} была отправлена ✔️\r\n" +
                    $"Ожидайте сообщение о подтверждении\r\n" +
                    $"Для того, чтобы оформить новую заявку - нажмите кнопку \"Пополнить\"\r\n";
                await client.SendMessageAsync(upd, user, message, InlineButtonMessage.GetFillButtons());

                // После этого отправить заявку в группу в зависимости от команды.
                var teams = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                var teamChatId = teams[$"Teams:{user.Team}"];
                var messageGroup = await application.ExecuteApplicationForTeamChannel(user, client, upd);
                var messageTelegram = await client.SendTextMessageAsync(teamChatId, messageGroup,
                    replyMarkup: InlineButtonMessage.GetApproveAndCancelledButtonsByAdmin(application),
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

                application.TelegramMessageId = messageTelegram.MessageId;
                await applicationRepository.UpdateAsync(application);
            }

            if (text.Contains(BotCommands.CancelledByUserCommand))
            {
                user.Command = UserCommands.Cancelled;
                var appId = text.Split("_")[1];
                var applicationRepository = scope.ServiceProvider.GetRequiredService<IRepository<Application>>();
                var application = await applicationRepository.GetByIdAsync(Guid.Parse(appId));
                application.State = State.Cancelled;
                await applicationRepository.UpdateAsync(application);

                await client.SendMessageAsync(upd, user, "Ваша заявка была отменена ❌", InlineButtonMessage.GetFillButtons());
            }

            if (text.Contains(BotCommands.ApprovedByAdminCommand))
            {
                user.Command = UserCommands.Approved;
                var appId = text.Split("_")[1];
                var applicationSrvc = scope.ServiceProvider.GetRequiredService<IApplicationService>();
                var application = await applicationSrvc.UpdateState(scope, State.Payed, appId);

                var userByApp = await SendMessageByApplicationUser(client, userRepository, application,
                    $"Ваша заявка №{application.Sequence} на пополнение была оплачена ✔️");

                var messageForEdit = await application.ExecuteApplicationForTeamChannel(userByApp, client, upd);
                if (application.TelegramMessageId != null)
                {
                    await client.EditMessageTextAsync(chatId, application.TelegramMessageId.Value,
                        messageForEdit,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                }
                
                await client.SendTextMessageAsync(chatId,
                        $"Заявка №{application.Sequence} была оплачена ✔️");
            }

            if (text.Contains(BotCommands.CancelledByAdminCommand))
            {
                user.Command = UserCommands.Cancelled;
                var appId = text.Split("_")[1];
                var applicationSrvc = scope.ServiceProvider.GetRequiredService<IApplicationService>();
                var application = await applicationSrvc.UpdateState(scope, State.Cancelled, appId);

                await client.SendTextMessageAsync(chatId,
                    $"Введите причину отклонения, скопировав данную сообщение: `#CLD#{application.Id}#CLD#` и вставив ее в начало сообщения.",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
            }

            if (text.Contains("#CLD#"))
            {
                string pattern = @"#CLD#(.*?)#CLD#";
                Match match = Regex.Match(text, pattern);

                if (match.Success)
                {
                    var appId = match.Groups[1].Value;
                    var message = text.Replace(appId, "").Replace("#CLD#", "");
                    var applicationSrvc = scope.ServiceProvider.GetRequiredService<IApplicationService>();
                    var application = await applicationSrvc.UpdateState(scope, State.Cancelled, appId, message);

                    var userByApp = await SendMessageByApplicationUser(client, userRepository, application,
                        $"Ваша заявка №{application.Sequence} на пополнение была отклонена ❌ \nпо причине: {message}");

                    var messageForEdit = await application.ExecuteApplicationForTeamChannel(userByApp, client, upd);
                    if (application.TelegramMessageId != null)
                    {
                        await client.EditMessageTextAsync(chatId, application.TelegramMessageId.Value,
                            messageForEdit,
                            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                    }

                    await client.SendTextMessageAsync(chatId,
                        $"Заявка №{application.Sequence} была отклонена ❌");
                }
                else
                {
                    await client.SendTextMessageAsync(chatId, $"Упс, что-то пошло не так.\r\n\n" +
                        $"Нажмите кнопку отклонить еще раз.");
                }
            }

            if (text.Contains(BotCommands.InUnderСonsiderationForTLCommand))
            {
                user.Command = UserCommands.InUnderСonsiderationTL;
                var appId = text.Split("_")[1];
                var applicationSrvc = scope.ServiceProvider.GetRequiredService<IApplicationService>();
                var application = await applicationSrvc.UpdateState(scope, State.InUnderСonsiderationForTL, appId);

                var userByApp = await SendMessageByApplicationUser(client, userRepository, application, 
                    $"Ваша заявка №{application.Sequence} находится на рассмотрении TL команды, ожидайте 👀");

                var messageForadmin = await application.ExecuteApplicationForTeamChannel(userByApp, client, upd);

                if (application.TelegramMessageId == null)
                {
                    await client.SendTextMessageAsync(chatId, messageForadmin);
                }
                else
                {
                    await client.EditMessageTextAsync(chatId, application.TelegramMessageId.Value, messageForadmin,
                        replyMarkup: InlineButtonMessage.GetApproveAndCancelledButtonsByAdmin(application),
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                }

            }

            await userRepository.UpdateAsync(user);
        }
    }

    private async Task ExecuteStart(ITelegramBotClient client, long chatId)
    {
        await client.SendTextMessageAsync(chatId, "Добро пожаловать. Укажите в какой команде вы находитесь и мы продолжим.",
                    replyMarkup: InlineButtonMessage.GetStartChooseTeamButtons());
    }

    private async Task<User> SendMessageByApplicationUser(ITelegramBotClient client, IRepository<User> repository, Application application, 
        string message)
    {
        var userByApp = await repository.GetByIdAsync(application.UserId);
        var messageTelegram = await client.SendTextMessageAsync(userByApp.ChatTelegramId, message);
        userByApp.LastMessageId = messageTelegram.MessageId;
        await repository.UpdateAsync(userByApp);
        return userByApp;
    }
}
