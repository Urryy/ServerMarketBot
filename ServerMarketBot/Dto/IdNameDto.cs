namespace ServerMarketBot.Dto;

public class IdNameDto
{
    public IdNameDto(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
}
