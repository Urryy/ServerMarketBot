namespace ServerMarketBot.Dto;

public class TeamsSettings
{
    public Dictionary<string, string> Teams { get; set; } = new Dictionary<string, string>();
    public string Inferno { get; set; } = default!;
    public string Blasters { get; set; } = default!;
    public string Redline { get; set; } = default!;
    public string Mercury { get; set; } = default!;
    public string Galaxy { get; set; } = default!;
    public string Juniors { get; set; } = default!;
}
