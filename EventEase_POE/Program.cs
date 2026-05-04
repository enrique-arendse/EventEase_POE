using EventEase_POE.Data;
using EventEase_POE.Models;
using EventEase_POE.Service;
using EventEase_POE.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(
		builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Azure Blob Storage Service
var azureStorageConnection = builder.Configuration.GetConnectionString("AzureStorageConnection");
var containerName = builder.Configuration["ConnectionStrings:AzureStorageContainerName"];
builder.Services.AddSingleton(new AzureStorageService(builder.Configuration));
builder.Services.AddSingleton(new BlobStorageService(azureStorageConnection, containerName));
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

// ✅ Enable Session
builder.Services.AddSession();

var app = builder.Build();


// SEED ADMIN 
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    // If no admin exists, create one with a known default password.
    if (!context.Users.Any(u => u.Role == "Admin"))
    {
        context.Users.Add(new User
        {
            Name = "Admin",
            Email = "admin@eventease.com",
            PasswordHash = PasswordHasher.HashPassword("Admin123"), // default seeded password
            Role = "Admin"
        });
        context.SaveChanges();
    }

    // If an admin exists but the stored password doesn't include salt (legacy), reset to the default so login works.
    var existingAdmin = context.Users.FirstOrDefault(u => u.Role == "Admin");
    if (existingAdmin != null && (string.IsNullOrEmpty(existingAdmin.PasswordHash) || !existingAdmin.PasswordHash.Contains(':')))
    {
        existingAdmin.PasswordHash = PasswordHasher.HashPassword("Admin123");
        context.SaveChanges();
    }
}



// ================= MIDDLEWARE PIPELINE =================

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();


app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();