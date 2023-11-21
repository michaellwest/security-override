using Microsoft.Extensions.DependencyInjection;
using Sitecore.Abstractions;
using Sitecore.Buckets.Security;
using Sitecore.Data.SqlServer;
using Sitecore.DependencyInjection;
using Sitecore.Security.AccessControl;

namespace So
{
    public class SecurityRuleAuthorizationProvider : BucketAuthorizationProvider
    {
        private ItemAuthorizationHelper _itemHelper;

        protected override ItemAuthorizationHelper ItemHelper
        {
            get { return _itemHelper; }
            set { _itemHelper = value; }
        }

        public SecurityRuleAuthorizationProvider(SqlServerDataApi api) : base(api)
        {
            _itemHelper = new SecurityRuleAuthorizationHelper(ServiceLocator.ServiceProvider.GetService<BaseAccessRightManager>(), ServiceLocator.ServiceProvider.GetService<BaseRolesInRolesManager>(), ServiceLocator.ServiceProvider.GetService<BaseItemManager>());
        }
    }
}
