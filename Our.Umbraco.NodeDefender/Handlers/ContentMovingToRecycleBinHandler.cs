using System;
using System.Linq;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;

namespace NodeDefender.Handlers
{
    public class ContentMovingToRecycleBinHandler : BaseHandler,
                                                    INotificationHandler<ContentMovingToRecycleBinNotification>
    {
        private readonly DenyOptions _denyOptionsDeleting;


        public ContentMovingToRecycleBinHandler(IOptions<NodeDefenderSettings> settings,
                                                IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
                                                IContentTypeService contentTypeService)
            : base(settings, backOfficeSecurityAccessor, contentTypeService)
        {
            _denyOptionsDeleting = settings.Value.DenyDelete;
        }

        public void Handle(ContentMovingToRecycleBinNotification notification)
        {
            if (IsCurrentUserAllowed())
                return;

            foreach (var moveEventInfo in notification.MoveInfoCollection)
            {
                var node = moveEventInfo.Entity;
                var errorMessage = GetErrorMessage();

                if (_denyOptionsDeleting.DoctypeAliases is not null
                    && _denyOptionsDeleting.DoctypeAliases.Contains(node.ContentType.Alias))
                    notification.CancelOperation(errorMessage);

                if (_denyOptionsDeleting.NodeKeys is not null
                    && _denyOptionsDeleting.NodeKeys.Contains(node.Key.ToString()))
                    notification.CancelOperation(errorMessage);

                if (_denyOptionsDeleting.CompositionAliases is not null)
                {
                    var contentType = ContentTypeService.Get(node.Id);
                    var compositions = contentType.CompositionAliases();

                    if (compositions.Any(x => _denyOptionsDeleting.CompositionAliases.Contains(x)))
                        notification.CancelOperation(errorMessage);
                }

                if (_denyOptionsDeleting.NodeIds is not null
                    && _denyOptionsDeleting.NodeIds.Contains(node.Id))
                    notification.CancelOperation(errorMessage);
            }
        }

        private EventMessage GetErrorMessage()
        {
            var category = _denyOptionsDeleting.Message?.Category ?? "Error";
            var message = _denyOptionsDeleting.Message?.Message ?? "You can not delete this node.";

            var tryParse = Enum.TryParse(_denyOptionsDeleting.Message?.Type, out EventMessageType eventMessageType);
            return new EventMessage(category, message, tryParse ? eventMessageType : EventMessageType.Error);
        }
    }
}