using Microsoft.Extensions.DependencyInjection;
using NodeDefender.Handlers;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;

namespace NodeDefender
{

    public class NodeDefender : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            // get all doctypes and node Ids
            builder.Services.Configure<NodeDefenderSettings>(builder.Config.GetSection(NodeDefenderSettings.NodeDefender));

            builder.AddNotificationHandler<ContentSavingNotification, ContentSavingHandler>();
            builder.AddNotificationHandler<ContentMovingToRecycleBinNotification, ContentMovingToRecycleBinHandler>();
        }
    }
}