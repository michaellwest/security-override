using Microsoft.Extensions.DependencyInjection;
using Sitecore;
using Sitecore.Abstractions;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using Sitecore.Security.AccessControl;
using Sitecore.SecurityModel;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace So
{
    public static class SecurityRuleManager
    {
        private static readonly ConcurrentDictionary<string, SecurityRuleEntry> _cachedSecurityRuleEntries =
            new ConcurrentDictionary<string, SecurityRuleEntry>();

        private static volatile Lazy<Item[]> _securityRuleItems = new Lazy<Item[]>(LoadSecurityRuleItems);

        public static void Invalidate()
        {
            _cachedSecurityRuleEntries.Clear();
            _securityRuleItems = new Lazy<Item[]>(LoadSecurityRuleItems);
        }

        private static Item[] LoadSecurityRuleItems()
        {
            using (new SecurityDisabler())
            {
                var database = ServiceLocator.ServiceProvider.GetRequiredService<BaseFactory>().GetDatabase("master", true);
                var securityRuleItems = database.GetItem(Constants.SecurityRulesRoot)?
                    .Axes.GetDescendants()
                    .Where(d => d.TemplateID == Templates.SecurityRule.Id)
                    .ToArray();

                if (securityRuleItems == null)
                {
                    Log.Warn($"A required item for {nameof(SecurityRuleManager)} is missing. ItemId: {Constants.SecurityRulesRoot}", typeof(SecurityRuleManager));
                    return Array.Empty<Item>();
                }

                return securityRuleItems;
            }
        }

        private static SecurityRuleEntry GetSecurityRuleCachedEntry(Item currentItem, Item securityRuleItem, Database database)
        {
            var cacheKey = $"{currentItem.ID}-{securityRuleItem.ID}";

            if (_cachedSecurityRuleEntries.TryGetValue(cacheKey, out var entry))
            {
                return entry;
            }

            var isEnabled = MainUtil.GetBool(securityRuleItem.Fields[Templates.SecurityRule.Fields.Enabled].Value, false);
            if (!isEnabled)
            {
                entry = new SecurityRuleEntry();
                _cachedSecurityRuleEntries.TryAdd(cacheKey, entry);
                return entry;
            }

            Log.Debug($"Evaluating SecurityRule {securityRuleItem.ID} for item {currentItem.ID}", nameof(SecurityRuleManager));

            var ruleContext = new RuleContext
            {
                Item = currentItem
            };

            var accessRules = securityRuleItem.Fields[Templates.SecurityRule.Fields.AccessRules].Value;

            var rules = RuleFactory.ParseRules<RuleContext>(database, accessRules);
            if (rules == null || !rules.Rules.Any() || rules.Rules.All(rule => !rule.Evaluate(ruleContext)))
            {
                entry = new SecurityRuleEntry();
                _cachedSecurityRuleEntries.TryAdd(cacheKey, entry);
                return entry;
            }

            var access = AccessRuleSerializer.Instance.Deserialize(securityRuleItem.Fields[Templates.SecurityRule.Fields.AccessRights].Value);

            entry = new SecurityRuleEntry
            {
                SecurityRuleItemId = securityRuleItem.ID,
                AccessRightsOverride = access,
                IsOverridden = true,
            };

            _cachedSecurityRuleEntries.TryAdd(cacheKey, entry);

            return entry;
        }

        public static AccessRuleCollection GetSecurityRules(Item currentItem)
        {
            Assert.ArgumentNotNull(currentItem, nameof(currentItem));

            var securityRuleItems = _securityRuleItems.Value;
            var database = ServiceLocator.ServiceProvider.GetRequiredService<BaseFactory>().GetDatabase("master", true);

            foreach (var securityRuleItem in securityRuleItems)
            {
                var entry = GetSecurityRuleCachedEntry(currentItem, securityRuleItem, database);
                if (entry != null && entry.IsOverridden)
                {
                    return entry.AccessRightsOverride;
                }
            }

            return null;
        }
    }

    internal class SecurityRuleEntry
    {
        public ID SecurityRuleItemId { get; set; }
        public AccessRuleCollection AccessRightsOverride { get; set; }
        public bool IsOverridden { get; internal set; }
    }
}
