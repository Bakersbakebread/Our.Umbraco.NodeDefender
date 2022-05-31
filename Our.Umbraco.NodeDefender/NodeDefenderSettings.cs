using System.Collections.Generic;
using Umbraco.Cms.Core.Events;

namespace NodeDefender
{
    public class NodeDefenderSettings
    {
        public IEnumerable<string> AllowedUserGroups { get; set; }
        public DenyOptions DenyRename { get; set; }

        public DenyOptions DenyDuplicate { get; set; }

        public DenyOptions DenyDelete { get; set; }
    }

    public class DenyOptions
    {
        public string[] DoctypeAliases { get; set; }

        public string[] CompositionAliases { get; set; }
        public int[] NodeIds { get; set; }
        public string[] NodeKeys { get; set; }

        public ErrorMessage Message { get; set; }
    }

    public class ErrorMessage
    {
        public string Category { get; set; }
        public string Message { get; set; }
        public string Type { get; set; } = EventMessageType.Error.ToString();
    }
}