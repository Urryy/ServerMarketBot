using ServerMarketBot.Commands;
using ServerMarketBot.Entities;
using ServerMarketBot.Entities.Common;
using Telegram.Bot.Types.ReplyMarkups;

namespace ServerMarketBot.Buttons;

public class InlineButtonMessage
{
    public static InlineKeyboardMarkup GetStartChooseTeamButtons(List<string> teams)
    {
        var keyboard = new List<List<InlineKeyboardButton>>();
        var rowCount = Math.Ceiling((double)(teams.Count / 2));

        for (int i = 0; i < rowCount; i++)
        {
            var row = new List<InlineKeyboardButton>();

            int index = i * 2;

            if(index < teams.Count)
            {
                row.Add(InlineKeyboardButton.WithCallbackData(teams[index], teams[index] + "Command"));
            }

            index++;

            if(index < teams.Count)
            {
                row.Add(InlineKeyboardButton.WithCallbackData(teams[index], teams[index] + "Command"));
            }

            keyboard.Add(row);
        }
        return new InlineKeyboardMarkup(keyboard);
    }

    public static InlineKeyboardMarkup GetFillButtons()
    {
        return new InlineKeyboardMarkup(new[]
{
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Пополнить", BotCommands.FillCommand),
            },
        });
    }

    public static InlineKeyboardMarkup GetTypesFillButtons()
    {
        return new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Агенты", BotCommands.AgentsCommand),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Ламанш", BotCommands.LamanshCommand),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Расходники", BotCommands.RashodnikiCommand),
            }
        });
    }

    public static InlineKeyboardMarkup GetTypesFillAgentsButtons()
    {
        return new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Premium Agency", BotCommands.PremiumAgency),
                InlineKeyboardButton.WithCallbackData("Luca", BotCommands.Luca),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Quang Cao", BotCommands.QuangCao),
                InlineKeyboardButton.WithCallbackData("SD Agency", BotCommands.SDAgency),
            },
        });
    }

    public static InlineKeyboardMarkup GetGroupsButtons()
    {
        return new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Group 1", BotCommands.Group1),
                InlineKeyboardButton.WithCallbackData("Group 2", BotCommands.Group2),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Group 3", BotCommands.Group3),
                InlineKeyboardButton.WithCallbackData("Group 4", BotCommands.Group4),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Group 5", BotCommands.Group5),
                InlineKeyboardButton.WithCallbackData("Group 6", BotCommands.Group6),
            }
        });
    }

    public static InlineKeyboardMarkup GetApproveAndCancelledButtonsByUser(Guid Id)
    {
        return new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Подтвердить ✔️", (BotCommands.ApprovedByUserCommand +"_"+ Id.ToString())),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Отменить ❌", (BotCommands.CancelledByUserCommand +"_"+ Id.ToString())),
            }
        });
    }

    public static InlineKeyboardMarkup GetSettingsButtons()
    {
        return new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Команда", ("/settings_" + BotCommands.TeamSettingsCommand))
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Агенты", ("/settings_" + BotCommands.AgentsSettingsCommand))
            }
        });
    }

    public static InlineKeyboardMarkup GetTeamSettings()
    {
        return new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Создать", ("/settings_" + BotCommands.TeamSettingsAddCommand))
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Изменить", ("/settings_" + BotCommands.TeamSettingsChangeCommand))
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Удалить", ("/settings_" + BotCommands.TeamSettingsDeleteCommand))
            }
        });
    }

    public static InlineKeyboardMarkup GetAgentsSettings()
    {
        return new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Создать", ("/settings_" + BotCommands.AgentsSettingsAddCommand))
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Изменить", ("/settings_" + BotCommands.AgentsSettingsChangeCommand))
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Удалить", ("/settings_" + BotCommands.AgentsSettingsDeleteCommand))
            }
        });
    }

    public static InlineKeyboardMarkup GetApproveAndCancelledButtonsByAdmin(Application app)
    {
        if(app.State != State.InUnderСonsiderationForTL)
        {
            return new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Оплачено ✔️", (BotCommands.ApprovedByAdminCommand +"_"+ app.Id.ToString())),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("На рассмотрении", (BotCommands.InUnderСonsiderationForTLCommand +"_"+ app.Id.ToString()))
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Отклонена ❌", $"{BotCommands.CancelledByAdminCommand}_{app.Id}")
                }
            });
        }
        else
        {
            return new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Оплачено ✔️", (BotCommands.ApprovedByAdminCommand +"_"+ app.Id.ToString())),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Отклонена ❌", $"{BotCommands.CancelledByAdminCommand}_{app.Id}")
                }
            });
        }
    }
}
