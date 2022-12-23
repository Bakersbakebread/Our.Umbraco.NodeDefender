using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;

namespace NodeDefender.Handlers;

public class SendingAllowedChildrenNotificationHandler : BaseHandler, INotificationHandler<SendingAllowedChildrenNotification>
{
    private readonly HashSet<string> _doctypeAliases;
    private readonly HashSet<int> _nodeIds;
    private readonly HashSet<string> _compositionAliases;

    public SendingAllowedChildrenNotificationHandler(IOptions<NodeDefenderSettings> settings,
        IBackOfficeSecurityAccessor backOfficeSecurityAccessor, IContentTypeService contentTypeService)
        : base(settings, backOfficeSecurityAccessor, contentTypeService)
    {
        var denyOptionsDuplicate = settings.Value.DenyDuplicate;

        _doctypeAliases = new HashSet<string>(denyOptionsDuplicate.DoctypeAliases);
        _nodeIds = new HashSet<int>(denyOptionsDuplicate.NodeIds);
        _compositionAliases = new HashSet<string>(denyOptionsDuplicate.CompositionAliases);
    }

    public void Handle(SendingAllowedChildrenNotification notification)
    {
        var children = notification.Children.ToList();
        children.RemoveAll(Match());
        notification.Children = children;
    }

    private Predicate<ContentTypeBasic> Match()
    {
        return node => _doctypeAliases.Contains(node.Alias)
                       || _nodeIds.Contains((int)node.Id!)
                       || _compositionAliases.Any(x =>
                           ContentTypeService.Get(node.Alias)?.CompositionAliases().Contains(x) == true);
    }
}