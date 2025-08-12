using System.Collections.ObjectModel;

namespace Infrastructure.Constants
{
   public record SchoolPermission(string Feature, string Action,string Description,string Group="", bool IsBasic=false, bool IsRoot=false)
    {
        public string Name => NameFor(Feature, Action);
        public static string NameFor(string feature, string action)=> $"Permission.{feature}.{action}";
    }

    public static class SchoolPermissions
    {
        private static readonly SchoolPermission[] _allPermissions =
        [
            
            // School tenant permissions
            new SchoolPermission(
                SchoolFeature.Tenants, SchoolAction.Create, "Create Tenants","Tenancy", IsRoot: true),
            new SchoolPermission(SchoolFeature.Tenants, SchoolAction.Read, "Read Tenants", "Tenancy", IsRoot: true),
            new SchoolPermission(SchoolFeature.Tenants, SchoolAction.Update, "Update Tenants","Tenancy", IsRoot: true),
            new SchoolPermission(SchoolFeature.Tenants, SchoolAction.UpgradeSubscription, "Upgrade Tenant's Subscription", "Tenancy", IsRoot: true),
            
            
            // school user permissions
            new SchoolPermission(SchoolFeature.Users,SchoolAction.Create,"Create Users","SystemAccess"),
            new SchoolPermission(SchoolFeature.Users,SchoolAction.Update,"Update Users","SystemAccess"),
            new SchoolPermission(SchoolFeature.Users,SchoolAction.Delete,"Delete Users","SystemAccess"),
            new SchoolPermission(SchoolFeature.Users,SchoolAction.Read,"Read Users","SystemAccess"),
            
            // school user roles
            new SchoolPermission(SchoolFeature.UserRoles,SchoolAction.Read,"Read Users Roles","SystemAccess"),
            new SchoolPermission(SchoolFeature.UserRoles,SchoolAction.Read,"Read Users Roles","SystemAccess"),
            
            // school roles permissions
            new SchoolPermission(SchoolFeature.Roles,SchoolAction.Create,"Create Roles","SystemAccess"),
            new SchoolPermission(SchoolFeature.Roles,SchoolAction.Read,"Read Roles","SystemAccess"),
            new SchoolPermission(SchoolFeature.Roles,SchoolAction.Update,"Update Roles","SystemAccess"),
            new SchoolPermission(SchoolFeature.Roles,SchoolAction.Delete,"Delete Roles","SystemAccess"),
            
            
            new SchoolPermission(SchoolFeature.RoleClaims,SchoolAction.Read,"Read Role Claims/Permissions","SystemAccess"),
            new SchoolPermission(SchoolFeature.RoleClaims,SchoolAction.Update,"Update Role Claims/Permissions","SystemAccess"),
            
            new SchoolPermission(SchoolFeature.Schools,SchoolAction.Read,"Read Schools","Academics",IsBasic:true),
            new SchoolPermission(SchoolFeature.Schools,SchoolAction.Create,"Create Schools","Academics"),
            new SchoolPermission(SchoolFeature.Schools,SchoolAction.Update,"Update Schools","Academics"),
            new SchoolPermission(SchoolFeature.Schools,SchoolAction.Delete,"Delete Schools","Academics"),
          
            new SchoolPermission(SchoolFeature.Tokens,SchoolAction.RefreshToken,"Generate Refresh Token","SystemAccess",IsBasic:true),
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
