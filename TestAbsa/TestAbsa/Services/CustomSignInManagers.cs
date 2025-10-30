using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using TestAbsa.Data;

namespace TestAbsa.Services
{
    public class CustomSignInManager : SignInManager<ApplicationUser>
    {
        public CustomSignInManager(
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<ApplicationUser>> logger,
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<ApplicationUser> confirmation)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
        }

        public override async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            var user = await UserManager.FindByNameAsync(userName);
            if (user == null)
            {
                return SignInResult.Failed;
            }

            // Check if user is approved (for employees)
            if (user.UserRole == "Employee" && !user.IsApproved)
            {
                Logger.LogWarning("User {UserId} attempted to sign in but is not approved.", user.Id);
                return SignInResult.NotAllowed;
            }

            return await base.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
        }

        public override async Task<SignInResult> CheckPasswordSignInAsync(ApplicationUser user, string password, bool lockoutOnFailure)
        {
            // Check if user is approved (for employees)
            if (user.UserRole == "Employee" && !user.IsApproved)
            {
                Logger.LogWarning("User {UserId} attempted to sign in but is not approved.", user.Id);
                return SignInResult.NotAllowed;
            }

            return await base.CheckPasswordSignInAsync(user, password, lockoutOnFailure);
        }
    }
}