using Umbraco.Cms.Core.Models;

namespace NodeDefender.Models;

public class DoctypeDto
{
    public DoctypeDto()
    {
    }
    
    public DoctypeDto(IContentType content)
    {
        HasError = false;
        Alias = content.Alias;
        Id = content.Id;
        Icon = content.Icon;
    }

    public bool HasError { get; set; }
    public string Alias { get; set; }
    public int Id { get; set; }
    public string Icon { get; set; }
}