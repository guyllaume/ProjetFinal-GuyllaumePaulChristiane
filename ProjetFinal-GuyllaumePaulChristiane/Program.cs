using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjetFinal_GuyllaumePaulChristiane.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ProjetFinal_GPC_DBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProjetFinal_GPC_DBContext") ?? throw new InvalidOperationException("Connection string 'ProjetFinal_GPC_DBContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthorization();

builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<ProjetFinal_GPC_DBContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapIdentityApi<IdentityUser>();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
