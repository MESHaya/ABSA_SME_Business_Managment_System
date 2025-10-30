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
            string[] roleNames = { "Admin", "Manager", "Employee" };

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

            // Create default organizations
            var absaOrg = await context.Organizations.FirstOrDefaultAsync(o => o.Name == "Absa System Organization")
                           ?? new Organization { Name = "Absa System Organization" };
            if (absaOrg.Id == 0)
            {
                context.Organizations.Add(absaOrg);
                await context.SaveChangesAsync();
                logger.LogInformation("Absa System Organization created.");
            }

            var defaultSMEOrg = await context.Organizations.FirstOrDefaultAsync(o => o.Name == "Default SME Client")
                                ?? new Organization { Name = "Default SME Client" };
            if (defaultSMEOrg.Id == 0)
            {
                context.Organizations.Add(defaultSMEOrg);
                await context.SaveChangesAsync();
                logger.LogInformation("Default SME Client organization created.");
            }

            // Create the admin user if it doesn't exist
            var adminEmail = "admin@testabsa.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    UserRole = "Admin",
                    IsApproved = true,
                    ApprovedDate = DateTime.UtcNow,
                    RegistrationDate = DateTime.UtcNow,
                    OrganizationId = absaOrg.Id
                };

                string adminPassword = "Admin@123";
                var createResult = await userManager.CreateAsync(admin, adminPassword);

                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                    logger.LogInformation("Admin user created successfully. Email: {Email}", adminEmail);
                }
                else
                {
                    logger.LogError("Failed to create Admin user: {Errors}",
                        string.Join(", ", createResult.Errors.Select(e => e.Description)));
                }
            }

            // (Optional) Create a default manager for SME clients
            var managerEmail = "manager@testabsa.com";
            var managerUser = await userManager.FindByEmailAsync(managerEmail);
            if (managerUser == null)
            {
                var manager = new ApplicationUser
                {
                    UserName = managerEmail,
                    Email = managerEmail,
                    EmailConfirmed = true,
                    UserRole = "Manager",
                    IsApproved = true,
                    ApprovedDate = DateTime.UtcNow,
                    RegistrationDate = DateTime.UtcNow,
                    OrganizationId = defaultSMEOrg.Id
                };

                string managerPassword = "Manager@123";
                var managerResult = await userManager.CreateAsync(manager, managerPassword);
                if (managerResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(manager, "Manager");
                    logger.LogInformation("Manager user created successfully: {Email}", managerEmail);
                }
                else
                {
                    logger.LogError("Failed to create Manager user: {Errors}",
                        string.Join(", ", managerResult.Errors.Select(e => e.Description)));
                }
            }
        }

        // Reusable method to add a Manager
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