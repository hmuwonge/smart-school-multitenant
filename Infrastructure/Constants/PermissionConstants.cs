using System.Collections.ObjectModel;

namespace Infrastructure.Constants
{
   public record SchoolPermission(string Feature, string Action,string Description,string Group="", bool IsBasic=false, bool IsRoot=false)
    {
        public string Name => NameFor(Action,Feature);
        public static string NameFor(string feature, string action)=> $"Permission.{feature}.{action}";
    }

    public static class SchoolPermissions
    {
        private static readonly SchoolPermission[] _allPermissions =
        [
            
            // School tenant permissions
            new SchoolPermission(
                SchoolAction.Create, SchoolFeature.Tenants, "Create Tenants","Tenancy", IsRoot: true),
            new SchoolPermission(SchoolAction.Read, SchoolFeature.Tenants, "Read Tenants", "Tenancy", IsRoot: true),
            new SchoolPermission(SchoolAction.Update, SchoolFeature.Tenants, "Update Tenants","Tenancy", IsRoot: true),
            new SchoolPermission(SchoolAction.UpgradeSubscription, SchoolFeature.Tenants, "Upgrade Tenant's Subscription", "Tenancy", IsRoot: true),
            
            
            // school user permissions
            new SchoolPermission(SchoolAction.Create,SchoolFeature.Users,"Create Users","SystemAccess"),
            new SchoolPermission(SchoolAction.Update,SchoolFeature.Users,"Update Users","SystemAccess"),
            new SchoolPermission(SchoolAction.Delete,SchoolFeature.Users,"Delete Users","SystemAccess"),
            new SchoolPermission(SchoolAction.Read,SchoolFeature.Users,"Read Users","SystemAccess"),
            
            // school user roles
            new SchoolPermission(SchoolAction.Read,SchoolFeature.UserRoles,"Read Users Roles","SystemAccess"),
            new SchoolPermission(SchoolAction.Read,SchoolFeature.UserRoles,"Read Users Roles","SystemAccess"),
            
            // school roles permissions
            new SchoolPermission(SchoolAction.Create,SchoolFeature.Roles,"Create Roles","SystemAccess"),
            new SchoolPermission(SchoolAction.Read,SchoolFeature.Roles,"Read Roles","SystemAccess"),
            new SchoolPermission(SchoolAction.Update,SchoolFeature.Roles,"Update Roles","SystemAccess"),
            new SchoolPermission(SchoolAction.Delete,SchoolFeature.Roles,"Delete Roles","SystemAccess"),
            
            
            new SchoolPermission(SchoolAction.Read,SchoolFeature.RoleClaims,"Read Role Claims/Permissions","SystemAccess"),
            new SchoolPermission(SchoolAction.Update,SchoolFeature.RoleClaims,"Update Role Claims/Permissions","SystemAccess"),
            
            new SchoolPermission(SchoolAction.Read,SchoolFeature.Schools,"Read Schools","Academics",IsBasic:true),
            new SchoolPermission(SchoolAction.Create,SchoolFeature.Schools,"Create Schools","Academics"),
            new SchoolPermission(SchoolAction.Update,SchoolFeature.Schools,"Update Schools","Academics"),
            new SchoolPermission(SchoolAction.Delete,SchoolFeature.Schools,"Delete Schools","Academics"),
        ];

        public static IReadOnlyList<SchoolPermission> All { get; } =
            new ReadOnlyCollection<SchoolPermission>(_allPermissions);

        public static IReadOnlyList<SchoolPermission> Root { get; } =
            new ReadOnlyCollection<SchoolPermission>(_allPermissions.Where(p => p.IsRoot).ToArray());
        
        public static IReadOnlyList<SchoolPermission> Admin { get; } =
            new ReadOnlyCollection<SchoolPermission>(_allPermissions.Where(p => !p.IsRoot).ToArray());
        
        public static IReadOnlyList<SchoolPermission> Basic { get; } =
            new ReadOnlyCollection<SchoolPermission>(_allPermissions.Where(p => p.IsBasic).ToArray());
        
    }
}
