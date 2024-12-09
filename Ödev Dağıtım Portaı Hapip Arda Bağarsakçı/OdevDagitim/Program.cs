using AspNetCoreHero.ToastNotification;
using OdevDagitim.Models;
using OdevDagitim.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped(typeof(GenericRepository<>));
builder.Services.AddScoped<UserRepository>();
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("sqlCon"));
});
builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 10;
    config.IsDismissable = true;
    config.Position = NotyfPosition.BottomRight;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opt =>
    {
        opt.Cookie.Name = "CookieAuthApp";
        opt.ExpireTimeSpan = TimeSpan.FromDays(3);
        opt.LoginPath = "/Home/Login";
        opt.LogoutPath = "/Home/Logout";
        opt.AccessDeniedPath = "/User/Login";
        opt.SlidingExpiration = false;
    });
builder.Services.AddScoped<ClassRepository>();
builder.Services.AddScoped<AssignmentRepository>();
builder.Services.AddScoped<AssignmentSubmissionRepository>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files")),
    RequestPath = "/files"
});

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=login}/{id?}");




app.Run();
