using Umbraco.Cms.Core.Dashboards;

namespace NodeDefender;

public class NodeDefenderDashboard : IDashboard
{
    public string Alias => "NodeDefenderDashboard";
    public string View => "/App_Plugins/Our.Umbraco.NodeDefender/index.html";
    public string[] Sections => new[] { "settings" };
    public IAccessRule[] AccessRules => System.Array.Empty<IAccessRule>();
}