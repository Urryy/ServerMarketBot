﻿using ServerMarketBot.Dto;
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
    public static string WildCommand = "WildCommand";


    public static string FillCommand = "FillCommand";

    public static string AgentsCommand = "AgentsCommand";
    public static string LamanshCommand = "LamanshCommand";
    public static string RashodnikiCommand = "RashodnikiCommand";

    public static string PremiumAgency = "Premium AgencyAgentCommand";
    public static string Luca = "LucaAgentCommand";
    public static string QuangCao = "QuangCao";
    public static string SDAgency = "SDAgency";

    public static string Group1 = "Group1";
    public static string Group2 = "Group2";
    public static string Group3 = "Group3";
    public static string Group4 = "Group4";
    public static string Group5 = "Group5";
    public static string Group6 = "Group6";

    public static string BotInfoServerAgentsPremiumAgencyGroupCommand = "Введите вручную название сервера и сумму пополнения, после чего отправьте данное сообщение (Например VPS1 2000$):";
    public static string BotInfoServerAgentsLucaCommand = "Введите вручную название сервера и сумму пополнения, после чего отправьте данное сообщение (Например P1 1000$)";
    public static string BotInfoServerAgentsQuangCaoCommand = "Введите вручную название сервера и сумму пополнения, после чего отправьте данное сообщение (Например S1 1000$):";
    public static string BotInfoServerLamanshCommand = "Введите сумму пополнения, после чего отправьте данное сообщение (Например 1000$):";
    public static string BotInfoServerRashodnikiCommand = "Введите вручную 1. Адрес кошелька/Ссылку на оплату/Контакт продавца у кого что-то купить/Данные для входа на сайт оплаты 2. Назначение платежа 3. Сумма, после чего отправьте данное сообщение:";

    public static string ApprovedByUserCommand = "ApprovedByUserCommand";
    public static string CancelledByUserCommand = "CancelledByUserCommand";

    public static string ApprovedByAdminCommand = "ApprovedByAdminCommand";
    public static string CancelledByAdminCommand = "CancelledByAdminCommand";
    public static string InUnderСonsiderationForTLCommand = "СonsiderationTLCommand";

    public static string TeamSettingsCommand = "TeamSettingsCommand";
    public static string AgentsSettingsCommand = "AgentsSettingsCommand";

    public static string TeamSettingsAddCommand = "TeamSettingsAddCommand";
    public static string TeamSettingsChangeCommand = "TeamSettingsChangeCommand";
    public static string TeamSettingsDeleteCommand = "TeamSettingsDeleteCommand";

    public static string AgentsSettingsAddCommand = "AgentsSettingsAddCommand";
    public static string AgentsSettingsChangeCommand = "AgentsSettingsChangeCommand";
    public static string AgentsSettingsDeleteCommand = "AgentsSettingsDeleteCommand";

    public static List<IdNameDto> GroupCommands = new List<IdNameDto>
    {
        new IdNameDto(1.ToString(), Group1),
        new IdNameDto(2.ToString(), Group2),
        new IdNameDto(3.ToString(), Group3),
        new IdNameDto(4.ToString(), Group4),
        new IdNameDto(5.ToString(), Group5),
        new IdNameDto(6.ToString(), Group6),
    };

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
