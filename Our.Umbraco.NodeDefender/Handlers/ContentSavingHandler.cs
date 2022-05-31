using System;
using System.Linq;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace NodeDefender.Handlers
{
    public class ContentSavingHandler : BaseHandler,
                                        INotificationHandler<ContentSavingNotification>
    {
        private readonly DenyOptions _denyOptionsRename;
        private readonly DenyOptions _denyOptionsDuplicate;

        public ContentSavingHandler(IOptions<NodeDefenderSettings> settings,
                                    IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
                                    IContentTypeService contentTypeService)
            : base(settings, backOfficeSecurityAccessor, contentTypeService)
        {
            _denyOptionsDuplicate = settings.Value.DenyDuplicate;
            _denyOptionsRename = settings.Value.DenyRename;
        }

        public void Handle(ContentSavingNotification notification)
        {
            if (IsCurrentUserAllowed())
                return;

            foreach (var node in notification.SavedEntities)
            {
                if (node.HasIdentity) // node is not new!
                    HandleRename(node, notification);
                else
                    HandleDuplicate(node, notification);
            }
        }

        private void HandleDuplicate(IContentBase node, ContentSavingNotification notification)
        {
            var errorMessage = GetDuplicateErrorMessage();
            
            if (_denyOptionsDuplicate.DoctypeAliases is not null
                && _denyOptionsDuplicate.DoctypeAliases.Contains(node.ContentType.Alias))
                notification.CancelOperation(errorMessage);
            
            if (_denyOptionsDuplicate.NodeKeys is not null
                && _denyOptionsDuplicate.NodeKeys.Contains(node.GetUdi().ToString()))
                notification.CancelOperation(errorMessage);

            if (_denyOptionsDuplicate.CompositionAliases is not null)
            {
                var contentType = ContentTypeService.Get(node.ContentTypeId);
                var compositions = contentType.CompositionAliases();

                if (compositions.Any(x => _denyOptionsDuplicate.CompositionAliases.Contains(x)))
                    notification.CancelOperation(errorMessage);
            }

            if (_denyOptionsDuplicate.NodeIds is not null
                && _denyOptionsDuplicate.NodeIds.Contains(node.Id))
                notification.CancelOperation(errorMessage);
        }

        private void HandleRename(IContentBase node, ContentSavingNotification notification)
        {
            var predicate = node.Id > 0 && node.IsPropertyDirty("Name");

            if (!predicate)
                return;

            var errorMessage = GetRenameErrorMessage();
            if (_denyOptionsRename.DoctypeAliases is not null
                && _denyOptionsRename.DoctypeAliases.Contains(node.ContentType.Alias))
                notification.CancelOperation(errorMessage);
            
            if (_denyOptionsRename.CompositionAliases is not null)
            {
                var contentType = ContentTypeService.Get(node.ContentTypeId);
                var compositions = contentType.CompositionAliases();
            
                if (compositions.Any(x => _denyOptionsRename.CompositionAliases.Contains(x)))
                    notification.CancelOperation(errorMessage);
            }
            
            if (_denyOptionsRename.NodeKeys is not null
                && _denyOptionsRename.NodeKeys.Contains(node.Key.ToString()))
                notification.CancelOperation(errorMessage);


            if (_denyOptionsRename.NodeIds is not null
                && _denyOptionsRename.NodeIds.Contains(node.Id))
                notification.CancelOperation(errorMessage);
        }


        private EventMessage GetRenameErrorMessage()
        {
            var category = _denyOptionsRename.Message?.Category ?? "Error";
            var message = _denyOptionsRename.Message?.Message ?? "You cannot rename this node.";

            var tryParse = Enum.TryParse(_denyOptionsRename.Message?.Type, out EventMessageType eventMessageType);
            return new EventMessage(category, message, tryParse ? eventMessageType : EventMessageType.Error);
        }

        private EventMessage GetDuplicateErrorMessage()
        {
            var category = _denyOptionsDuplicate.Message?.Category ?? "Error";
            var message = _denyOptionsDuplicate.Message?.Message ?? "You cannot create more than one of these nodes.";

            var tryParse = Enum.TryParse(_denyOptionsDuplicate.Message?.Type, out EventMessageType eventMessageType);
            return new EventMessage(category, message, tryParse ? eventMessageType : EventMessageType.Error);
        }
    }
}