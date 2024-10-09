using ServerMarketBot.Entities.Common;

namespace ServerMarketBot.Entities;

public class User : Entity
{
    public override Guid Id { get; set; } = Guid.NewGuid();
    public long TelegramId { get; set; }
    public long ChatTelegramId { get; set; }
    public string Name { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public Role Role { get; set; }
    public string Team { get; set; }
    public int? LastMessageId { get; set; }
    public string Command { get; set; }

    protected User() 
        : base()
    {

    }

    public User(long telegramId, long chatTelegramId, string name, Role role, string command, int? lastMessageId = null)
        : this()
    {
        TelegramId = telegramId;
        ChatTelegramId = chatTelegramId;
        Name = name;
        Role = role;
        LastMessageId = lastMessageId;
        Team = string.Empty;
        Command = command;
    }
}
