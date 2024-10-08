using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using ProjetFinal_GuyllaumePaulChristiane.Data;
using ProjetFinal_GuyllaumePaulChristiane.Tasks;
using ProjetFinal_GuyllaumePaulChristiane.Models;
var builder = WebApplication.CreateBuilder(args);

// Enregistrer l'impl�mentation de IEmailSender
builder.Services.AddSingleton<IEmailSender, EmailSender>();

builder.Services.AddDbContext<ProjetFinal_GPC_DBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProjetFinal_GPC_DBContext") ?? throw new InvalidOperationException("Connection string 'ProjetFinal_GPC_DBContext' not found.")));

// Configure Identity with default User inherits IdentityUser
builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ProjetFinal_GPC_DBContext>()
    .AddDefaultTokenProviders();

// Configurer les options de cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddAuthorization();

builder.Services.AddScoped<EmailSender>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Middleware d'authentification et d'autorisation
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=DVDs}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
