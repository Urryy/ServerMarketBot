using ServerMarketBot.Dto;

namespace ServerMarketBot.Entities.Common;

public enum TypeApplication
{
    None,
    Agents,
    Lamansh,
    Rashodniki
}

public static class TypeApplicationExtension
{
    public static List<IdNameDto> TypesApplication = new List<IdNameDto>
    {
        new IdNameDto(TypeApplication.Agents.ToString(), "Агенты"),
        new IdNameDto(TypeApplication.Lamansh.ToString(), "Ламанш"),
        new IdNameDto(TypeApplication.Rashodniki.ToString(), "Расходники")
    };

    public static string GetNameTypeApplication(this TypeApplication typeApplication)
    {
        return TypesApplication.First(i => i.Id == typeApplication.ToString()).Name;
    }
}
