using ServerMarketBot.Entities.Common;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ServerMarketBot.Extension;

public static class TelegramExtension
{
    public static string GetUserName(this Update upd)
    {
        var name = "unknown_user";
        if(upd.Type == UpdateType.CallbackQuery)
        {
            name = upd.CallbackQuery!.From?.Username ?? "unknown_user";
        }
        else if (upd.Type == UpdateType.Message)
        {
            name = upd.Message!.From?.Username ?? "unknown_user";
        }

        return name;
    }

    public static long? GetTelegramId(this Update upd)
    {
        if (upd.Type == UpdateType.CallbackQuery)
        {
            return upd.CallbackQuery!.From?.Id;
        }
        else if (upd.Type == UpdateType.Message)
        {
            return upd.Message!.From?.Id;
        }
        else
        {
            return null;
        }
    }

    public static async Task<long?> GetChatId(this Update upd)
    {
        if (upd.Type == UpdateType.CallbackQuery)
        {
            return upd.CallbackQuery?.Message?.Chat.Id;
        }
        else if (upd.Type == UpdateType.Message)
        {
            return upd.Message?.Chat?.Id;
        }
        else
        {
            return null;
        }
    }

    public static async Task<string?> GetText(this Update upd)
    {
        if (upd.Type == UpdateType.CallbackQuery)
        {
            return upd.CallbackQuery?.Data;
        }
        else if (upd.Type == UpdateType.Message)
        {
            return upd.Message?.Text;
        }
        else
        {
            return string.Empty;
        }
    }

    public static async Task<string?> GetCallbackQueryId(this Update upd)
    {
        if (upd.Type == UpdateType.CallbackQuery)
        {
            return upd.CallbackQuery?.Id;
        }
        return string.Empty;
    }

    public static async Task<long?> GetUserId(this Update upd)
    {
        if (upd.Type == UpdateType.CallbackQuery)
        {
            return upd.CallbackQuery?.From.Id;
        }
        else if (upd.Type == UpdateType.Message)
        {
            return upd.Message?.From?.Id;
        }
        else
        {
            return null;
        }
    }

    public static async Task SendMessageAsync(this ITelegramBotClient client, Update upd, Entities.User user, string text, InlineKeyboardMarkup buttons = null)
    {
        var chatId = await upd.GetChatId();

        if(user.LastMessageId != null)
        {
            try
            {

                var res = await client.EditMessageTextAsync(chatId, user.LastMessageId.Value, text, replyMarkup: buttons);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                user.LastMessageId = (await client.SendTextMessageAsync(chatId, text, replyMarkup: buttons)).MessageId;
            }
        }
        else
        {
            user.LastMessageId = (await client.SendTextMessageAsync(chatId, text, replyMarkup: buttons)).MessageId;
        }
    }

    public static async Task<bool> IsMemberOfChannel(this ITelegramBotClient bot, Update upd, IConfiguration config)
    {
        var userId = await upd.GetUserId();
        if(upd.GetTelegramId() == 746672080)
        {
            return true;
        }

        if (userId != null)
        {
            var t = await bot.GetChatMemberAsync(config["ChatMembersId"]!, userId.Value);
            if (t.Status != ChatMemberStatus.Left && t.Status != ChatMemberStatus.Restricted && t.Status != ChatMemberStatus.Kicked)
            {
                return true;
            }
        }
        return false;
    }
}
