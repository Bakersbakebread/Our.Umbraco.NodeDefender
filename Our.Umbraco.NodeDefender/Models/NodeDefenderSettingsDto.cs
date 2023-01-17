using System.Collections.Generic;

namespace NodeDefender.Models;

public class NodeDefenderSettingsDto
{
    public List<AllowedUserGroupsDto> AllowedUserGroups { get; set; }
    public NodeDefenderSettings Raw { get; set; }

    public List<DenyOptionsDto> DenyOptionsDtos { get; set; }
}