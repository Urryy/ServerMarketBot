namespace ServerMarketBot.Entities.Common;

public class Dictionary
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string DictionaryKey { get; set; } = default!;
    public Dictionary(string dictionaryKey)
    {
        DictionaryKey = dictionaryKey;
    }
}
