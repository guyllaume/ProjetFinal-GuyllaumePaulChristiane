using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjetFinal_GuyllaumePaulChristiane.Data;
using ProjetFinal_GuyllaumePaulChristiane.Tasks;
var builder = WebApplication.CreateBuilder(args);

// Enregistrer l'implémentation de IEmailSender
builder.Services.AddSingleton<IEmailSender, EmailSender>();

builder.Services.AddDbContext<ProjetFinal_GPC_DBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProjetFinal_GPC_DBContext") ?? throw new InvalidOperationException("Connection string 'ProjetFinal_GPC_DBContext' not found.")));

// Configure Identity with default IdentityUser
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ProjetFinal_GPC_DBContext>()
    .AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddAuthorization();

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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
