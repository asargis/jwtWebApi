using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MyApp.Models;

namespace MyApp.Seeder
{
    public static class MyAppDataInitializer
    {
        public static async Task SeedData(MyAppContext context, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            context.Database.EnsureCreated();

            if (!roleManager.RoleExistsAsync("user").Result)
            {
                AppRole role = new AppRole("user");

                await roleManager.CreateAsync(role);
            }

            if (!roleManager.RoleExistsAsync("admin").Result)
            {
                AppRole role = new AppRole("admin");

                await roleManager.CreateAsync(role);
            }

            if (userManager.FindByNameAsync("7900000000").Result == null)
            {
                AppUser user = new AppUser();
                user.UserName = "7900000000";
                user.Email = "admin@admin.com";
                user.PhoneNumber = user.UserName;

                var result = await userManager.CreateAsync(user, "qwerty");

                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, "qwerty");
                    await userManager.AddToRoleAsync(user, "admin");
                }
            }

            if (userManager.FindByNameAsync("79111111111").Result == null)
            {
                AppUser user = new AppUser();
                user.UserName = "79111111111";
                user.Email = "user@user.com";

                IdentityResult result = await Task.FromResult(userManager.CreateAsync(user, "qwerty")).Result;

                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, "123456");
                    Task.FromResult(await userManager.AddToRoleAsync(user, "user")).Wait();
                }
            }
        }
    }
}
