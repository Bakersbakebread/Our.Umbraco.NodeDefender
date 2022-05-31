using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Security;

namespace NodeDefender.Handlers
{
    public class BaseHandler
    {
        private readonly IEnumerable<string> _allowedUserGroups;
        private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;

        protected BaseHandler(IOptions<NodeDefenderSettings> settings, IBackOfficeSecurityAccessor backOfficeSecurityAccessor)
        {
            _backOfficeSecurityAccessor = backOfficeSecurityAccessor;
            _allowedUserGroups = settings.Value.AllowedUserGroups;
        }

        protected bool IsCurrentUserAllowed()
        {
            if (_allowedUserGroups is null)
                return false;
            
            var user = _backOfficeSecurityAccessor.BackOfficeSecurity.CurrentUser;
            var userGroups = user.Groups;
            return userGroups.Any(x => _allowedUserGroups.Contains(x.Alias));
        }
    }
}