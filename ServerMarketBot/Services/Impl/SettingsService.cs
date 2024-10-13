using ServerMarketBot.Buttons;
using ServerMarketBot.Commands;
using ServerMarketBot.Entities;
using ServerMarketBot.Extension;
using ServerMarketBot.Repository.Interfaces;
using ServerMarketBot.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using User = ServerMarketBot.Entities.User;

namespace ServerMarketBot.Services.Impl;

public class SettingsService : ISettingsService
{
    private IServiceProvider _provider { get; set; }
    public SettingsService(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task SignTeam(ITelegramBotClient client, Update upd)
    {
        using(var scope = _provider.CreateScope())
        {
            var chatId = await upd.GetChatId();
            try
            {
                var text = await upd.GetText();
                var teamName = text.Split("#")[1];
                var team = await scope.ServiceProvider.GetRequiredService<IRepository<Team>>().GetByExpressionAsync(i => i.Name == teamName);
                team.ChatTeamId = chatId;
                await scope.ServiceProvider.GetRequiredService<IRepository<Team>>().UpdateAsync(team);
                await client.SendTextMessageAsync(chatId, "Команда успешно завязана на данный чат");
            }
            catch (Exception)
            {
                await client.SendTextMessageAsync(chatId, "Упс. При подвязке команды что-то пошло не так");
            }
        }    
    }

    public async Task ExecuteSettings(ITelegramBotClient client, Update upd)
    {
        using (var scope = _provider.CreateScope())
        {
            var chatId = await upd.GetChatId();
            var text = await upd.GetText();
            if (string.IsNullOrEmpty(text) || chatId == null) return;

            var userRepository = scope.ServiceProvider.GetRequiredService<IRepository<User>>();
            var telegramId = upd.GetTelegramId();
            var user = await userRepository.GetByExpressionAsync(i => i.TelegramId == telegramId);
            if (user == null) return;

            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var isMember = await client.IsMemberOfChannel(upd, configuration);
            if (!isMember) return;

            if (configuration["ChatMembersId"] == chatId.ToString()) return;

            if (text == "/settings")
            {
                user.Command = UserCommands.Settings;
                var message = await client.SendTextMessageAsync(chatId, "Пункт меню администрации.\nВыберите тип конфигурации.",
                    replyMarkup: InlineButtonMessage.GetSettingsButtons());
                user.LastMessageId = message.MessageId;
            }

            if (text.Contains(BotCommands.TeamSettingsCommand))
            {
                user.Command = UserCommands.TeamSettings;
                await client.SendMessageAsync(upd, user, "Выберите одну из опций",
                    InlineButtonMessage.GetTeamSettings());
            }

            if (text.Contains(BotCommands.TeamSettingsAddCommand))
            {
                user.Command = UserCommands.TeamAddSettings;
                await client.SendMessageAsync(upd, user, "Впишите название команды");
            }

            if (!text.Contains(BotCommands.TeamSettingsAddCommand) && user.Command == UserCommands.TeamAddSettings)
            {
                user.Command = UserCommands.Start;
                var team = new Team(text);
                await scope.ServiceProvider.GetRequiredService<IRepository<Team>>().AddAsync(team);
                var message = await client.SendTextMessageAsync(chatId, "Команда успешно была добавлена. Теперь чтобы связать команду с Чатом, вам следует зайти в чат" +
                    $"и прописать скопировав команду `/sign_team#{text}`", 
                    replyMarkup: InlineButtonMessage.GetBackSettingsButtons(),
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                user.LastMessageId = message.MessageId;
            }

            if (text.Contains(BotCommands.TeamSettingsDeleteCommand))
            {
                user.Command = UserCommands.TeamDeleteSettings;
                var teams = (await scope.ServiceProvider.GetRequiredService<IRepository<Team>>().GetAllAsync()).Select(i => i.Name).ToList();
                await client.SendMessageAsync(upd, user, "Выберите команду, которую стоит удалить",
                    InlineButtonMessage.GetTeamSettingsButtonsByCommand(teams, "deleteteam"));
            }

            if (text.Contains("deleteteam"))
            {
                user.Command = UserCommands.Start;
                var name = text.Split("_")[1];
                var team = await scope.ServiceProvider.GetRequiredService<IRepository<Team>>().GetByExpressionAsync(i => i.Name == name);
                await scope.ServiceProvider.GetRequiredService<IRepository<Team>>().DeleteAsync(team);
                await client.SendMessageAsync(upd, user, "Команда успешно была удалена",
                     InlineButtonMessage.GetBackSettingsButtons());
            }

            if (text.Contains(BotCommands.TeamSettingsChangeCommand))
            {
                user.Command = UserCommands.TeamChangeSettings;
                var teams = (await scope.ServiceProvider.GetRequiredService<IRepository<Team>>().GetAllAsync()).Select(i => i.Name).ToList();
                await client.SendMessageAsync(upd, user, "Выберите команду, которую стоит изменить",
                    InlineButtonMessage.GetTeamSettingsButtonsByCommand(teams, "changeteam"));
            }

            if (text.Contains("changeteam"))
            {
                var name = text.Split("_")[1];
                user.Command = UserCommands.TeamChangeSuccessSettings + $"_{name}";
                await client.SendMessageAsync(upd, user, "Впишите новое название для команды");
            }

            if (!text.Contains("changeteam") && user.Command.Contains(UserCommands.TeamChangeSuccessSettings))
            {
                var choosesTeam = user.Command.Split("_")[1];
                user.Command = UserCommands.Start;
                var team = await scope.ServiceProvider.GetRequiredService<IRepository<Team>>().GetByExpressionAsync(i => i.Name == choosesTeam);
                team.Name = text;
                await scope.ServiceProvider.GetRequiredService<IRepository<Team>>().UpdateAsync(team);
                await client.SendMessageAsync(upd, user, "Команда успешно была изменена",
                    InlineButtonMessage.GetBackSettingsButtons());
            }

            if (text.Contains(BotCommands.AgentsSettingsCommand))
            {
                user.Command = UserCommands.AgentsSettings;
                await client.SendMessageAsync(upd, user, "Выберите одну из опций",
                    InlineButtonMessage.GetAgentsSettings());
            }

            if (text.Contains(BotCommands.AgentsSettingsAddCommand))
            {
                user.Command = UserCommands.AgentsAddSettings;
                await client.SendMessageAsync(upd, user, "Впишите название Агента");
            }

            if (!text.Contains(BotCommands.AgentsSettingsAddCommand) && user.Command == UserCommands.AgentsAddSettings)
            {
                user.Command = UserCommands.Start;
                var agent = new Agent(text);
                await scope.ServiceProvider.GetRequiredService<IRepository<Agent>>().AddAsync(agent);
                var message = await client.SendTextMessageAsync(chatId, "Новый агент был успешно добавлен", replyMarkup: InlineButtonMessage.GetBackSettingsButtons());
                user.LastMessageId = message.MessageId;
            }

            if (text.Contains(BotCommands.AgentsSettingsDeleteCommand))
            {
                user.Command = UserCommands.AgentsDeleteSettings;
                var agents = (await scope.ServiceProvider.GetRequiredService<IRepository<Agent>>().GetAllAsync()).Select(i => i.Name).ToList();
                await client.SendMessageAsync(upd, user, "Выберите агента, которого стоит удалить",
                    InlineButtonMessage.GetTeamSettingsButtonsByCommand(agents, "deleteagent"));
            }

            if (text.Contains("deleteagent"))
            {
                user.Command = UserCommands.Start;
                var name = text.Split("_")[1];
                var agent = await scope.ServiceProvider.GetRequiredService<IRepository<Agent>>().GetByExpressionAsync(i => i.Name == name);
                await scope.ServiceProvider.GetRequiredService<IRepository<Agent>>().DeleteAsync(agent);
                await client.SendMessageAsync(upd, user, "Агент успешно был удален",
                     InlineButtonMessage.GetBackSettingsButtons());
            }

            if (text.Contains(BotCommands.AgentsSettingsChangeCommand))
            {
                user.Command = UserCommands.AgentsChangeSettings;
                var agents = (await scope.ServiceProvider.GetRequiredService<IRepository<Agent>>().GetAllAsync()).Select(i => i.Name).ToList();
                await client.SendMessageAsync(upd, user, "Выберите aгента, которого стоит изменить",
                    InlineButtonMessage.GetTeamSettingsButtonsByCommand(agents, "changeagent"));
            }

            if (text.Contains("changeagent"))
            {
                var name = text.Split("_")[1];
                user.Command = UserCommands.AgentsChangeSuccessSettings + $"_{name}";
                await client.SendMessageAsync(upd, user, "Впишите новое название для агента");
            }

            if(!text.Contains("changeagent") && user.Command.Contains(UserCommands.AgentsChangeSuccessSettings))
            {
                var choosesAgent = user.Command.Split("_")[1];
                user.Command = UserCommands.Start;
                var agent = await scope.ServiceProvider.GetRequiredService<IRepository<Agent>>().GetByExpressionAsync(i => i.Name == choosesAgent);
                agent.Name = text;
                await scope.ServiceProvider.GetRequiredService<IRepository<Agent>>().UpdateAsync(agent);
                await client.SendMessageAsync(upd, user, "Агент успешно был изменен",
                    InlineButtonMessage.GetBackSettingsButtons());
            }

            await userRepository.UpdateAsync(user);
        }
    }
}
