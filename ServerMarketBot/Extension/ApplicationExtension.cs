using ServerMarketBot.Buttons;
using ServerMarketBot.Entities;
using ServerMarketBot.Entities.Common;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ServerMarketBot.Extension;

using User = Entities.User;

public static class ApplicationExtension
{
    public static async Task ExecuteApplicationAgent(this Application app, User user, ITelegramBotClient client, Update upd)
    {
        var chatId = await upd.GetChatId();
        var message = $"Перепроверьте вашу заявку:\r\n\n" +
            $"Заявка №{app.Sequence} на пополнение:\r\n" +
            $"Type: {app.Type.GetNameTypeApplication()}\r\n" +
            $"Agent: {app.Agent.ToString()}\r\n" +
            $"Server and sum: {app.Message}\r\n\n" +
            $"Подтвердите или отклоните заявку";

        var messageTelegram = await client.SendTextMessageAsync(chatId, message,
            replyMarkup: InlineButtonMessage.GetApproveAndCancelledButtonsByUser(app.Id));

        user.LastMessageId = messageTelegram.MessageId;
    }

    public static async Task ExecuteApplicationAgentWithGroup(this Application app, User user, ITelegramBotClient client, Update upd)
    {
        var chatId = await upd.GetChatId();
        var message = $"Перепроверьте вашу заявку:\r\n\n" +
            $"Заявка №{app.Sequence} на пополнение:\r\n" +
            $"Type: {app.Type.GetNameTypeApplication()}\r\n" +
            $"Agent: {app.Agent.ToString()}\r\n" +
            $"Group: {app.Group}\r\n" +
            $"Server and sum: {app.Message}\r\n\n" +
            $"Подтвердите или отклоните заявку";

        var messageTelegram = await client.SendTextMessageAsync(chatId, message,
            replyMarkup: InlineButtonMessage.GetApproveAndCancelledButtonsByUser(app.Id));

        user.LastMessageId = messageTelegram.MessageId;
    }

    public static async Task ExecuteApplicationLamansh(this Application app, User user, ITelegramBotClient client, Update upd)
    {
        var chatId = await upd.GetChatId();
        var message = $"Перепроверьте вашу заявку:\r\n\n" +
            $"Заявка №{app.Sequence} на пополнение:\r\n" +
            $"Type: {app.Type.GetNameTypeApplication()}\r\n" +
            $"Sum: {app.Message}\r\n\n" +
            $"Подтвердите или отклоните заявку";

        var messageTelegram = await client.SendTextMessageAsync(chatId, message,
            replyMarkup: InlineButtonMessage.GetApproveAndCancelledButtonsByUser(app.Id));

        user.LastMessageId = messageTelegram.MessageId;
    }

    public static async Task ExecuteApplicationRashodniki(this Application app, User user, ITelegramBotClient client, Update upd)
    {
        var chatId = await upd.GetChatId();
        var message = $"Перепроверьте вашу заявку:\r\n\n" +
            $"Заявка №{app.Sequence} на пополнение:\r\n" +
            $"Type: {app.Type.GetNameTypeApplication()}\r\n" +
            $"Данные: {app.Message}\r\n\n" +
            $"Подтвердите или отклоните заявку";

        var messageTelegram = await client.SendTextMessageAsync(chatId, message,
            replyMarkup: InlineButtonMessage.GetApproveAndCancelledButtonsByUser(app.Id));

        user.LastMessageId = messageTelegram.MessageId;
    }

    public static async Task<string> ExecuteApplicationForTeamChannel(this Application app, User user, ITelegramBotClient client, Update upd)
    {
        if(app.Agent == AgentApplication.PremiumAgency)
        {
            return $"Заявка №{app.Sequence} от пользователя @{user.Name.Replace("_", "\\_")} на пополнение:\r\n" +
            $"- Type: {app.Type.GetNameTypeApplication()}\r\n" +
            $"- Team: {user.Team.ToString()}\r\n" +
            $"`Premium Agency | Group{app.Group} | {app.Message.Replace("_", "\\_")}`";
        }
        else if(app.Agent == AgentApplication.Luca)
        {
            return $"Заявка №{app.Sequence} от пользователя @{user.Name.Replace("_", "\\_")} на пополнение:\r\n" +
            $"- Type: {app.Type.GetNameTypeApplication()}\r\n" +
            $"- Agent: Luca\r\n" +
            $"`{user.Team.ToString()} | {app.Message.Replace("_", "\\_")}`";
        }
        else if (app.Agent == AgentApplication.QuangCao || app.Agent == AgentApplication.SDAgency)
        {
            return $"Заявка №{app.Sequence} от пользователя @{user.Name.Replace("_", "\\_")} на пополнение:\r\n" +
            $"- Type: {app.Type.GetNameTypeApplication()}\r\n" +
            $"- Team: {user.Team.ToString()}\r\n" +
            $"`{app.Agent} | {app.Message.Replace("_", "\\_")}`";
        }
        else if(app.Type == TypeApplication.Rashodniki)
        {
            return $"Заявка №{app.Sequence} от пользователя @{user.Name.Replace("_", "\\_")} на пополнение:\r\n" +
                $"- Type: {app.Type.GetNameTypeApplication()}\r\n" +
                $"- Team: {user.Team.ToString()}\r\n" +
                $"{app.Message.Replace("_", "\\_")}";
        }
        else
        {
            return $"Заявка №{app.Sequence} от пользователя @{user.Name.Replace("_", "\\_")} на пополнение:\r\n" +
            $"- Type: {app.Type.GetNameTypeApplication()}\r\n" +
            $"- Team: {user.Team.ToString()}\r\n" +
            $"`{app.Message.Replace("_", "\\_")}`";
        }
    }
}
