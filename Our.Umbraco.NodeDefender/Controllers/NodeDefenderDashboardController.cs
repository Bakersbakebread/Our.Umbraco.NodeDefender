using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NodeDefender.Models;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.Common.Controllers;


namespace NodeDefender.Controllers;

[PluginController("NodeDefender")]
public class NodeDefenderDashboardController : UmbracoApiController
{
    private readonly NodeDefenderSettings _denyOptions;
    private readonly IUserService _userService;
    private readonly IContentTypeService _contentTypeService;

    public NodeDefenderDashboardController(IOptions<NodeDefenderSettings> denyOptions, IUserService userService,
        IContentTypeService contentTypeService)
    {
        _userService = userService;
        _contentTypeService = contentTypeService;
        _denyOptions = denyOptions.Value;
    }

    public async Task<string> GetNodeDefenderSettings()
    {
        // map the settings to a dto
        var dto = new NodeDefenderSettingsDto
        {
            AllowedUserGroups = GetAllowedUserGroupsDto(_denyOptions.AllowedUserGroups),
            DenyOptionsDtos = GetDenyOptionsDto(_denyOptions),
            Raw = _denyOptions
        };
        return JsonConvert.SerializeObject(dto);
    }

    private List<DenyOptionsDto> GetDenyOptionsDto(NodeDefenderSettings denyOptions)
    {
        var listOfDenyOptions = new List<DenyOptionsDto>();

        var denyDeleteOptions = new DenyOptionsDto
        {
            Type = "Delete",
            Message = denyOptions.DenyDelete.Message?.Message,
            Doctype = GetDoctypeDtos(denyOptions.DenyDelete)
        };

        var denyRenameOptions = new DenyOptionsDto
        {
            Type = "Rename",
            Message = denyOptions.DenyRename.Message?.Message,
            Doctype = GetDoctypeDtos(denyOptions.DenyRename)
        };

        var denyDuplicateOptions = new DenyOptionsDto
        {
            Type = "Duplicate",
            Message = denyOptions.DenyDuplicate.Message?.Message,
            Doctype = GetDoctypeDtos(denyOptions.DenyDuplicate)
        };

        listOfDenyOptions.Add(denyRenameOptions);
        listOfDenyOptions.Add(denyDeleteOptions);
        listOfDenyOptions.Add(denyDuplicateOptions);

        return listOfDenyOptions;
    }

    private List<DoctypeDto> GetDoctypeDtos(DenyOptions denyOptions)
    {
        var listOfDoctypeDtos = new List<DoctypeDto>();
        var contentTypes = _contentTypeService.GetAll().ToList();

        var valuesToCheck = GetValuesToCheck(denyOptions);

        // Iterate through the combined list and create a DoctypeDto for each value
        foreach (var value in valuesToCheck)
        {
            IContentType content = null;
            var doctypeDto = null as DoctypeDto;
            if (value is int id)
            {
                // value is an integer, check for a content type with a matching id
                content = contentTypes.FirstOrDefault(x => x.Id == id);
                if (content == null)
                {
                    doctypeDto = new DoctypeDto { HasError = true, Id = id };
                }
            }
            
            else if (value is string alias)
            {
                // value is a string, check for a content type with a matching alias or composition alias
                content = contentTypes.FirstOrDefault(x => x.Alias == alias || x.CompositionAliases().Contains(alias));
                if (content == null)
                {
                    doctypeDto = new DoctypeDto { HasError = true, Alias = alias };
                }
            }
            
            doctypeDto ??= new DoctypeDto(content);
            
            listOfDoctypeDtos.Add(doctypeDto);
        }

        return listOfDoctypeDtos;
    }

    private static List<object> GetValuesToCheck(DenyOptions denyOptions)
    {
        var idsToCheck = denyOptions.NodeIds;
        var aliasesToCheck = denyOptions.DoctypeAliases;
        var compositionAliasesToCheck = denyOptions.CompositionAliases;

        // Combine the lists of ids and aliases to check into a single list
        var valuesToCheck = new List<object>();

        if (idsToCheck != null)
        {
            valuesToCheck.AddRange(idsToCheck.Select(x => (object)x));
        }

        if (aliasesToCheck != null)
        {
            valuesToCheck.AddRange(aliasesToCheck);
        }

        if (compositionAliasesToCheck != null)
        {
            valuesToCheck.AddRange(compositionAliasesToCheck);
        }

        return valuesToCheck;
    }


    private List<AllowedUserGroupsDto> GetAllowedUserGroupsDto(IEnumerable<string> denyOptionsAllowedUserGroups)
    {
        var userGroups = _userService.GetAllUserGroups();
        var userGroupLookup = userGroups.ToDictionary(ug => ug.Alias, ug => ug);

        var list = new List<AllowedUserGroupsDto>();
        foreach (var allowedUserGroup in denyOptionsAllowedUserGroups)
        {
            // Look up the user group in the lookup table
            if (!userGroupLookup.TryGetValue(allowedUserGroup, out var userGroup)) continue;

            var dto = new AllowedUserGroupsDto
            {
                Alias = userGroup.Alias,
                ID = userGroup.Id,
                Name = userGroup.Name,
                Icon = userGroup.Icon
            };
            list.Add(dto);
        }

        return list;
    }
}