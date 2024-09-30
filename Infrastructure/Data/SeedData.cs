using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public static class SeedData
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "Member"};
            bool hasRolesBeenSeeded = await roleManager.RoleExistsAsync("Admin"); 

            if (!hasRolesBeenSeeded)
            {
                foreach (var roleName in roleNames)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
        

    }
}
