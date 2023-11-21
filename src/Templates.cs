using Sitecore.Data;

namespace So
{
    public static class Templates
    {
        public struct SecurityRule
        {
            public static ID Id = ID.Parse("{6E2BD501-9144-4C08-A1A2-0184319DF4E9}");

            public readonly struct Fields
            {
                public static readonly ID Enabled = new ID("{28088570-C24D-4DAF-86A6-1B714449DAC0}");
                public static readonly ID AccessRules = new ID("{34A7444A-68DC-42A7-8913-AD1241EAC707}");
                public static readonly ID AccessRights = new ID("{269B1F8F-EA0E-48F8-A19A-190EC7A5453A}");
            }
        }
    }
}
