using ServerMarketBot.Dto;
using ServerMarketBot.Entities.Common;

namespace ServerMarketBot.Commands;

public static class BotCommands
{
    public static string InfernoCommand = "InfernoCommand";
    public static string BlastersCommand = "BlastersCommand";
    public static string RedlineCommand = "RedlineCommand";
    public static string MercuryCommand = "MercuryCommand";
    public static string GalaxyCommand = "GalaxyCommand";
    public static string JuniorsCommand = "JuniorsCommand";

    public static string FillCommand = "FillCommand";

    public static string AgentsCommand = "AgentsCommand";
    public static string LamanshCommand = "LamanshCommand";
    public static string RashodnikiCommand = "RashodnikiCommand";

    public static string PremiumAgency = "PremiumAgency";
    public static string Luca = "Luca";
    public static string QuangCao = "QuangCao";

    public static string Group1 = "Group1";
    public static string Group2 = "Group2";
    public static string Group3 = "Group3";
    public static string Group4 = "Group4";
    public static string Group5 = "Group5";
    public static string Group6 = "Group6";

    public static string BotInfoServerAgentsPremiumAgencyGroupCommand = "Введите вручную название сервера и сумму пополнения, после чего отправьте данное сообщение (Например VPS1 2000$):";
    public static string BotInfoServerAgentsLucaCommand = "Введите вручную название сервера и сумму пополнения, после чего отправьте данное сообщение (Например P1 1000$)";
    public static string BotInfoServerAgentsQuangCaoCommand = "Введите вручную название сервера и сумму пополнения, после чего отправьте данное сообщение (Например S1 1000$):";

    public static List<IdNameDto> TeamCommands = new List<IdNameDto>
    {
        new IdNameDto(Team.Inferno.ToString(), InfernoCommand),
        new IdNameDto(Team.Blasters.ToString(), BlastersCommand),
        new IdNameDto(Team.Redline.ToString(), RedlineCommand),
        new IdNameDto(Team.Mercury.ToString(), MercuryCommand),
        new IdNameDto(Team.Galaxy.ToString(), GalaxyCommand),
        new IdNameDto(Team.Juniors.ToString(), JuniorsCommand),
    };

    public static List<IdNameDto> GroupCommands = new List<IdNameDto>
    {
        new IdNameDto(1.ToString(), Group1),
        new IdNameDto(2.ToString(), Group2),
        new IdNameDto(3.ToString(), Group3),
        new IdNameDto(4.ToString(), Group4),
        new IdNameDto(5.ToString(), Group5),
        new IdNameDto(6.ToString(), Group6),
    };

    public static bool IsTeamCommand(this string command)
    {
        return TeamCommands.Exists(i => i.Name == command);
    }

    public static bool IsGroupCommand(this string command)
    {
        return GroupCommands.Exists(i => i.Name == command);
    }

    public static int GetGroup(this string command)
    {
        var group = GroupCommands.First(i => i.Name == command).Id;
        var value = int.Parse(group);
        return value;
    }
}
