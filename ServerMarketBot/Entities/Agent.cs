namespace ServerMarketBot.Entities;

public class Agent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }

}
