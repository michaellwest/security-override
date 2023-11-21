using Sitecore.Abstractions;
using Sitecore.Data.Items;
using Sitecore.Security.AccessControl;

namespace So
{
    public class SecurityRuleAuthorizationHelper : ItemAuthorizationHelper
    {
        public SecurityRuleAuthorizationHelper(BaseAccessRightManager accessRightManager, BaseRolesInRolesManager rolesInRolesManager, BaseItemManager itemManager) : base(accessRightManager, rolesInRolesManager, itemManager)
        {
        }

        public override AccessRuleCollection GetAccessRules(Item item)
        {
            return SecurityRuleManager.GetSecurityRules(item) ?? base.GetAccessRules(item);
        }
    }
}
