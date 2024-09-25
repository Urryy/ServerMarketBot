using ServerMarketBot.Entities.Common;

namespace ServerMarketBot.Entities;

public class User : Entity
{
    public override Guid Id { get; set; } = Guid.NewGuid();
    public long ChatTelegramId { get; set; }
    public string Name { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public Role Role { get; set; }
    public long? LastMessageId { get; set; }

    protected User() 
        : base()
    {

    }

    public User(long chatTelegramId, string name, Role role, long? lastMessageId = null)
        : this()
    {
        ChatTelegramId = chatTelegramId;
        Name = name;
        Role = role;
        LastMessageId = lastMessageId;
    }
}
