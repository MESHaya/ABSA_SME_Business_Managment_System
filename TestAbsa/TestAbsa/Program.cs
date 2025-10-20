using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TestAbsa.Client.Pages;
using TestAbsa.Components;
using TestAbsa.Components.Account;
using TestAbsa.Services;
using TestAbsa.Data; //  Contains TestAbsa.Data.ApplicationUser

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
  .AddInteractiveServerComponents()
  .AddInteractiveWebAssemblyComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

// Register your Inventory Service
builder.Services.AddScoped<TestAbsa.Services.IInventoryService, TestAbsa.Services.InventoryService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    // FIX #1: This needs to be ApplicationScheme for local user login
    options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
})
  .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// FIX: Fully qualify ApplicationUser as TestAbsa.Data.ApplicationUser to resolve the ambiguity.
builder.Services.AddIdentityCore<TestAbsa.Data.ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
  .AddEntityFrameworkStores<ApplicationDbContext>()
  .AddSignInManager()
  .AddDefaultTokenProviders();

// FIX: Fully qualify ApplicationUser here too to match the identity setup type.
builder.Services.AddSingleton<IEmailSender<TestAbsa.Data.ApplicationUser>, IdentityNoOpEmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
  .AddInteractiveServerRenderMode()
  .AddInteractiveWebAssemblyRenderMode()
  .AddAdditionalAssemblies(typeof(TestAbsa.Client._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
