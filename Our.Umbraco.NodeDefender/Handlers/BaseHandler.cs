using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;

namespace NodeDefender.Handlers
{
    public class BaseHandler
    {
        private readonly IEnumerable<string> _allowedUserGroups;
        private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;
        public readonly IContentTypeService ContentTypeService;

        protected BaseHandler(IOptions<NodeDefenderSettings> settings,
                              IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
                              IContentTypeService contentTypeService)
        {
            _backOfficeSecurityAccessor = backOfficeSecurityAccessor;
            ContentTypeService = contentTypeService;
            _allowedUserGroups = settings.Value.AllowedUserGroups;
        }

        protected bool IsCurrentUserAllowed()
        {
            if (_allowedUserGroups is null)
                return false;

            //  we return true here because if you schedule publish, the validation is done at that point
            if (_backOfficeSecurityAccessor.BackOfficeSecurity == null)
                return true;

            var user = _backOfficeSecurityAccessor.BackOfficeSecurity.CurrentUser;
            var userGroups = user.Groups;
            return userGroups.Any(x => _allowedUserGroups.Contains(x.Alias));
        }
    }
}