namespace ServerMarketBot.Entities.Common;

public class DictionaryValue
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string DictionaryKey { get; set; } = default!;
    public string Value { get; set; } = default!;

    public DictionaryValue(string dictionaryKey, string value)
    {
        DictionaryKey = dictionaryKey;
        Value = value;
    }
}
