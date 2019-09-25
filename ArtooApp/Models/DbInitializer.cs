using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace Artoo.Models
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext context, RoleManager<IdentityRole> roleManager, AppUserManager userManager)
        {
            context.Database.EnsureCreated();
            SeedRoles(roleManager);
            SeedUser(userManager);
        }

        public static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("Administrator").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Administrator";
                //role.Description = "Perform all the operations.";
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }

            if (!roleManager.RoleExistsAsync("Factory Manager").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Factory Manager";
                //role.Description  = "Perform factory operations.";
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }

            if (!roleManager.RoleExistsAsync("Decathlon Manager").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Decathlon Manager";
                //role.Description = "Perform Decathlon operations.";
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }

            if (!roleManager.RoleExistsAsync("QPL").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "QPL";
                //role.Description = "Perform QPL operations.";
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }
        }

        public static void SeedUser(UserManager<ApplicationUser> userManager)
        {
            var userApp = userManager.FindByNameAsync("gmadmin").Result;
            if (userApp == null)
            {
                ApplicationUser user = new ApplicationUser();
                user.UserName = "gmadmin";
                user.Email = "gmadmin@artooapp.net";
                user.TenantId = 2;

                IdentityResult result = userManager.CreateAsync
                (user, "Admin123.").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user,
                                        "Administrator").Wait();
                }
            }
        }
    }
}
