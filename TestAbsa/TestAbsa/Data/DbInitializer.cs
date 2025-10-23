using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TestAbsa.Data;
using TestAbsa.Data.Models;

namespace TestAbsa.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            // Create roles if they don't exist
            string[] roleNames = { "Manager", "Employee" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                    if (result.Succeeded)
                    {
                        logger.LogInformation("Role {RoleName} created successfully.", roleName);
                    }
                    else
                    {
                        logger.LogError("Failed to create role {RoleName}. Errors: {Errors}",
                            roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
            }

            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // 1. Create a default organization if none exists
            var defaultOrg = await context.Organizations.FirstOrDefaultAsync(o => o.Name == "Default Absa SME Client");
            if (defaultOrg == null)
            {
                defaultOrg = new Organization { Name = "Default Absa SME Client" };
                context.Organizations.Add(defaultOrg);
                await context.SaveChangesAsync();
                logger.LogInformation("Default organization created.");
            }

            // Create initial admin/manager if it doesn't exist
            var adminEmail = "admin@testabsa.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    UserRole = "Manager",
                    IsApproved = true,
                    ApprovedDate = DateTime.UtcNow,
                    RegistrationDate = DateTime.UtcNow,
                    //  Links Admin to the Default Organization --- should we have it as a default or should we have a unique one?
                     OrganizationId = defaultOrg.Id // Link to the newly created/found organization
                };

                string adminPassword = "Admin@123"; // Change this to a secure password
                var createResult = await userManager.CreateAsync(admin, adminPassword);

                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Manager");
                    logger.LogInformation("Admin user created successfully. Email: {Email}", adminEmail);
                    logger.LogWarning("IMPORTANT: Change the admin password immediately after first login!");
                }
                else
                {
                    logger.LogError("Failed to create admin user. Errors: {Errors}",
                        string.Join(", ", createResult.Errors.Select(e => e.Description)));
                }
            }
        }

        // Method to add additional managers programmatically
        public static async Task<IdentityResult> AddManagerAsync(
            UserManager<ApplicationUser> userManager,
            string email,
            string password)
        {
            var existingUser = await userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "A user with this email already exists."
                });
            }

            var manager = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                UserRole = "Manager",
                IsApproved = true,
                ApprovedDate = DateTime.UtcNow,
                RegistrationDate = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(manager, password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(manager, "Manager");
            }

            return result;
        }
    }
}