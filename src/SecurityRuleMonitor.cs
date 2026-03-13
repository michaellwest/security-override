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

        private void InvalidateCaches()
        {
            var cacheManager = ServiceLocator.ServiceProvider.GetRequiredService<BaseCacheManager>();
            cacheManager.GetAccessResultCache().Clear();
            SecurityRuleManager.Invalidate();
        }

        internal void OnItemSaved(object sender, EventArgs args)
        {
            if (EventDisabler.IsActive) return;

            Assert.ArgumentNotNull(args, "args");
            try
            {
                var item = Event.ExtractParameter<Item>(args, 0);
                if (IsMonitoredItem(item))
                {
                    InvalidateCaches();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error in {nameof(OnItemSaved)}", ex, this);
            }
        }

        internal void OnItemDeleting(object sender, EventArgs args)
        {
            if (EventDisabler.IsActive) return;

            Assert.ArgumentNotNull(args, "args");
            try
            {
                var item = Event.ExtractParameter<Item>(args, 0);
                if (IsMonitoredItem(item))
                {
                    InvalidateCaches();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error in {nameof(OnItemDeleting)}", ex, this);
            }
        }

        internal void OnRoleCreated(object sender, EventArgs args)
        {
            if (EventDisabler.IsActive) return;

            Assert.ArgumentNotNull(args, "args");
            try
            {
                var roleName = Event.ExtractParameter<string>(args, 0);
                if (string.IsNullOrEmpty(roleName)) return;

                InvalidateCaches();
            }
            catch (Exception ex)
            {
                Log.Error($"Error in {nameof(OnRoleCreated)}", ex, this);
            }
        }

        internal void OnRoleRemoved(object sender, EventArgs args)
        {
            if (EventDisabler.IsActive) return;

            Assert.ArgumentNotNull(args, "args");
            try
            {
                var roleName = Event.ExtractParameter<string>(args, 0);
                if (string.IsNullOrEmpty(roleName)) return;

                InvalidateCaches();
            }
            catch (Exception ex)
            {
                Log.Error($"Error in {nameof(OnRoleRemoved)}", ex, this);
            }
        }

        internal void OnRolesInRolesAltered(object sender, EventArgs args)
        {
            if (EventDisabler.IsActive) return;

            Assert.ArgumentNotNull(args, "args");
            try
            {
                var roles = Event.ExtractParameter<IEnumerable<Role>>(args, 0);
                if (roles == null) return;

                InvalidateCaches();
            }
            catch (Exception ex)
            {
                Log.Error($"Error in {nameof(OnRolesInRolesAltered)}", ex, this);
            }
        }

        internal void OnRolesInRolesRemoved(object sender, EventArgs args)
        {
            if (EventDisabler.IsActive) return;

            Assert.ArgumentNotNull(args, "args");
            try
            {
                var roleName = Event.ExtractParameter<string>(args, 0);
                if (string.IsNullOrEmpty(roleName)) return;

                InvalidateCaches();
            }
            catch (Exception ex)
            {
                Log.Error($"Error in {nameof(OnRolesInRolesRemoved)}", ex, this);
            }
        }

        internal void OnUserCreated(object sender, EventArgs args)
        {
            if (EventDisabler.IsActive) return;

            Assert.ArgumentNotNull(args, "args");
            try
            {
                var user = Event.ExtractParameter<MembershipUser>(args, 0);
                if (user == null || string.IsNullOrEmpty(user.UserName)) return;

                InvalidateCaches();
            }
            catch (Exception ex)
            {
                Log.Error($"Error in {nameof(OnUserCreated)}", ex, this);
            }
        }

        internal void OnUserRemoved(object sender, EventArgs args)
        {
            if (EventDisabler.IsActive) return;

            Assert.ArgumentNotNull(args, "args");
            try
            {
                var username = Event.ExtractParameter<string>(args, 0);
                if (string.IsNullOrEmpty(username)) return;

                InvalidateCaches();
            }
            catch (Exception ex)
            {
                Log.Error($"Error in {nameof(OnUserRemoved)}", ex, this);
            }
        }

        internal void OnUserUpdated(object sender, EventArgs args)
        {
            if (EventDisabler.IsActive) return;

            Assert.ArgumentNotNull(args, "args");
            try
            {
                var user = Event.ExtractParameter<MembershipUser>(args, 0);
                if (user == null || string.IsNullOrEmpty(user.UserName)) return;

                InvalidateCaches();
            }
            catch (Exception ex)
            {
                Log.Error($"Error in {nameof(OnUserUpdated)}", ex, this);
            }
        }

        internal void OnRoleReferenceUpdated(object sender, EventArgs args)
        {
            if (EventDisabler.IsActive) return;

            Assert.ArgumentNotNull(args, "args");
            try
            {
                var data = Event.ExtractParameter<object>(args, 0);
                var stringArray = data as string[];
                if (stringArray == null || stringArray.Length == 0) return;
                var username = stringArray[0];
                if (string.IsNullOrEmpty(username)) return;

                InvalidateCaches();
            }
            catch (Exception ex)
            {
                Log.Error($"Error in {nameof(OnRoleReferenceUpdated)}", ex, this);
            }
        }
    }
}
