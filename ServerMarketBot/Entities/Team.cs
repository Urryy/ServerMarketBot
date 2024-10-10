using ServerMarketBot.Entities.Common;

namespace ServerMarketBot.Entities;

public class Team : Entity
{
    public override Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = default!;
    public long? ChatTeamId { get; set; }

    public Team(string name)
    {
        Name = name;
    }
}
