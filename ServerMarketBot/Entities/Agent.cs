using ServerMarketBot.Entities.Common;

namespace ServerMarketBot.Entities;

public class Agent : Entity
{
    public override Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public Agent(string name)
    {
        Name = name;
    }
}
