using Microsoft.Extensions.DependencyInjection;
using Sitecore.Abstractions;
using Sitecore.Data.Events;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.Events;
using Sitecore.Security.Accounts;
using System;
using System.Collections.Generic;
using System.Web.Security;

namespace So
{
    public class SecurityRuleMonitor
    {
        protected bool IsMonitoredItem(Item item)
        {
            return item != null &&
                    item.TemplateID == Templates.SecurityRule.Id &&
                    item.Paths.LongID.IndexOf(Constants.SecurityRulesRoot.ToString(), StringComparison.OrdinalIgnoreCase) > 0;
        }

        internal void OnItemSaved(object sender, EventArgs args)
        {
            if (EventDisabler.IsActive) return;

            Assert.ArgumentNotNull(args, "args");
            var item = Event.ExtractParameter<Item>(args, 0);
            if (IsMonitoredItem(item))
            {
                var cacheManager = ServiceLocator.ServiceProvider.GetRequiredService<BaseCacheManager>();
                cacheManager.GetAccessResultCache().Clear();

                SecurityRuleManager.Invalidate();
            }
        }

        internal void OnItemDeleting(object sender, EventArgs args)
        {
            if (EventDisabler.IsActive) return;

            Assert.ArgumentNotNull(args, "args");
            var item = Event.ExtractParameter<Item>(args, 0);
            if (IsMonitoredItem(item))
            {
                var cacheManager = ServiceLocator.ServiceProvider.GetRequiredService<BaseCacheManager>();
                cacheManager.GetAccessResultCache().Clear();

                SecurityRuleManager.Invalidate();
            }
        }

        internal void OnRoleCreated(object sender, EventArgs args)
        {
            if (EventDisabler.IsActive) return;

            Assert.ArgumentNotNull(args, "args");
            var roleName = Event.ExtractParameter<string>(args, 0);
            if (string.IsNullOrEmpty(roleName)) return;

            SecurityRuleManager.Invalidate();
        }

        internal void OnRoleRemoved(object sender, EventArgs args)
        {
            if (EventDisabler.IsActive) return;

            Assert.ArgumentNotNull(args, "args");
            var roleName = Event.ExtractParameter<string>(args, 0);
            if (string.IsNullOrEmpty(roleName)) return;

            SecurityRuleManager.Invalidate();
        }

        internal void OnRolesInRolesAltered(object sender, EventArgs args)
        {
            if (EventDisabler.IsActive) return;

            Assert.ArgumentNotNull(args, "args");
            var roles = Event.ExtractParameter<IEnumerable<Role>>(args, 0);
            if (roles == null) return;

            SecurityRuleManager.Invalidate();
        }

        internal void OnRolesInRolesRemoved(object sender, EventArgs args)
        {
            if (EventDisabler.IsActive) return;

            Assert.ArgumentNotNull(args, "args");
            var roleName = Event.ExtractParameter<string>(args, 0);
            if (string.IsNullOrEmpty(roleName)) return;

            SecurityRuleManager.Invalidate();
        }

        internal void OnUserCreated(object sender, EventArgs args)
        {
            if (EventDisabler.IsActive) return;

            Assert.ArgumentNotNull(args, "args");
            var user = Event.ExtractParameter<MembershipUser>(args, 0);
            if (user == null || string.IsNullOrEmpty(user.UserName)) return;

            SecurityRuleManager.Invalidate();
        }

        internal void OnUserRemoved(object sender, EventArgs args)
        {
            if (EventDisabler.IsActive) return;

            Assert.ArgumentNotNull(args, "args");
            var username = Event.ExtractParameter<string>(args, 0);
            if (string.IsNullOrEmpty(username)) return;

            SecurityRuleManager.Invalidate();
        }

        internal void OnUserUpdated(object sender, EventArgs args)
        {
            if (EventDisabler.IsActive) return;

            Assert.ArgumentNotNull(args, "args");
            var user = Event.ExtractParameter<MembershipUser>(args, 0);
            if (user == null || string.IsNullOrEmpty(user.UserName)) return;

            SecurityRuleManager.Invalidate();
        }

        internal void OnRoleReferenceUpdated(object sender, EventArgs args)
        {
            if (EventDisabler.IsActive) return;

            Assert.ArgumentNotNull(args, "args");
            var data = Event.ExtractParameter<object>(args, 0);
            if (data == null) return;
            var username = ((string[])data)[0];
            if (string.IsNullOrEmpty(username)) return;

            SecurityRuleManager.Invalidate();
        }
    }
}
