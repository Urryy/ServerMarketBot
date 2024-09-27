using ServerMarketBot.Entities.Common;

namespace ServerMarketBot.Entities;

public class Application : Entity
{
    public override Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public TypeApplication Type { get; set; }
    public AgentApplication Agent { get; set; }
    public State State { get; set; } = State.Draft;
    public int? Group { get; set; }
    public int Sequence { get; set; }
    public string Message { get; set; }
    public string? ModerationMessage { get; set; } // Если заявка отклонена через модерацию.
    public int? TelegramMessageId { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    protected Application()
    {

    }

    public Application(Guid userId, TypeApplication type, AgentApplication agent, int sequence, string message, int? group = null)
        : this()
    {
        UserId = userId;
        Type = type;
        Agent = agent;
        Group = group;
        Sequence = sequence;
        Message = message;
    }
}
