using ServerMarketBot.Buttons;
using ServerMarketBot.Commands;
using ServerMarketBot.Entities;
using ServerMarketBot.Extension;
using ServerMarketBot.Repository.Interfaces;
using ServerMarketBot.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = ServerMarketBot.Entities.User;

namespace ServerMarketBot.Services.Impl;

public class SettingsService : ISettingsService
{
    private IServiceProvider _provider { get; set; }
    public SettingsService(IServiceProvider provider)
    {
        _provider = provider;
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
                await client.SendMessageAsync(upd, user, "Пункт меню администрации.\nВыберите тип конфигурации.",
                    InlineButtonMessage.GetSettingsButtons());
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
                user.Command = UserCommands.TeamAddSuccessSettings;
                var team = new Team(text);
                await scope.ServiceProvider.GetRequiredService<IRepository<Team>>().AddAsync(team);
                await client.SendTextMessageAsync(chatId, "Команда успешно была добавлена. Теперь чтобы связать команду с Чатом, вам следует зайти в чат" +
                    $"и прописать команду /sign_team#{text}", replyMarkup: InlineButtonMessage.GetSettingsButtons());
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
                var name = text.Split("_")[1];
                var team = await scope.ServiceProvider.GetRequiredService<IRepository<Team>>().GetByExpressionAsync(i => i.Name == name);
                await scope.ServiceProvider.GetRequiredService<IRepository<Team>>().DeleteAsync(team);
                await client.SendMessageAsync(upd, user, "Команда успешно была удалена",
                     InlineButtonMessage.GetSettingsButtons());
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

            if (!text.Contains(BotCommands.TeamSettingsChangeCommand) && user.Command == UserCommands.TeamChangeSettings)
            {
                var choosesTeam = user.Command.Split("_")[1];
                var team = await scope.ServiceProvider.GetRequiredService<IRepository<Team>>().GetByExpressionAsync(i => i.Name == choosesTeam);
                team.Name = text;
                await scope.ServiceProvider.GetRequiredService<IRepository<Team>>().UpdateAsync(team);
                await client.SendMessageAsync(upd, user, "Команда успешно была изменена",
                    InlineButtonMessage.GetSettingsButtons());
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
                user.Command = UserCommands.AgentsAddSuccessSettings;
                var agent = new Agent(text);
                await scope.ServiceProvider.GetRequiredService<IRepository<Agent>>().AddAsync(agent);
                await client.SendTextMessageAsync(chatId, "Новый агент был успешно добавлен", replyMarkup: InlineButtonMessage.GetSettingsButtons());
            }

            await userRepository.UpdateAsync(user);
        }
    }
}
