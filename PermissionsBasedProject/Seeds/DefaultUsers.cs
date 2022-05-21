using Microsoft.AspNetCore.Identity;
using PermissionsBasedProject.Constants;

namespace PermissionsBasedProject.Seeds
{
    public static class DefaultUsers
    {
        public static async Task SeedBasicUserAsync(UserManager<IdentityUser> userManager)
        {
            var defaultUser = new IdentityUser
            {
                UserName = "basicuser@domain.com",
                Email = "basicuser@domain.com",
                EmailConfirmed = true
            };
            var user = await userManager.FindByEmailAsync(defaultUser.Email);
            if(user == null)
            {
                await userManager.CreateAsync(defaultUser , "Admin123*");
                await userManager.AddToRoleAsync(defaultUser,Roles.Basic.ToString() );
            }
        }



        public static async Task SeedSuperAdminAsync(UserManager<IdentityUser> userManager ,  RoleManager<IdentityRole> roleManager)
        {
            var superAdmin = new IdentityUser
            {
                UserName = "superadmin@domain.com",
                Email = "superadmin@domain.com",
                EmailConfirmed = true
            };
            var user = await userManager.FindByEmailAsync(superAdmin.Email);
            if (user == null)
            {
                await userManager.CreateAsync(superAdmin, "Admin123*");
                await userManager.AddToRolesAsync(superAdmin, new List<string> { Roles.Basic.ToString(), Roles.Admin.ToString() , Roles.SuperAdmin.ToString()});
            }

            await roleManager.SeedClaimsForSuperAdmin();
        }

        public static async Task SeedClaimsForSuperAdmin( this RoleManager<IdentityRole> roleManager)
        {
            var adminRole = await roleManager.FindByNameAsync(Roles.SuperAdmin.ToString());
            await roleManager.AddPermissionClaims(adminRole, "Products");
        }


        public static async Task AddPermissionClaims(this RoleManager<IdentityRole> roleManager , IdentityRole role , string module)
        {
            var allClaims = await roleManager.GetClaimsAsync(role);
            var allPermissions = Permissions.GeneratePermissionList(module);

             foreach (var permission in allPermissions)
             {
                if (!allClaims.Any(c => c.Type == Permissions.PermissionName && c.Value == permission))
                    await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim(Permissions.PermissionName, permission));
             }

        }
    }
}
