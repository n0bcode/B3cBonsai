using B3cBonsai.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.DataAccess.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Facebook;
using B3cBonsai.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Configuration;
using B3cBonsai.DataAccess.DbInitializer;
using Microsoft.Extensions.Options;
using B3cBonsai.Utility;
using B3cBonsai.Utility.Helper;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using B3cBonsaiWeb.Services;

namespace B3cBonsaiWeb
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            builder.Services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });

            // Add DbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString"));
            });

            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });

            // Tải secrets từ AWS Secrets Manager
            var secretsService = new SecretsManagerService(builder.Configuration);
            var authenticationSecrets = await secretsService.GetSecretAsync(); // Sử dụng await ở đây

            builder.Services.AddAuthentication()
                .AddCookie()
                .AddFacebook(FacebookDefaults.AuthenticationScheme, options =>
                {
                    options.ClientId = authenticationSecrets["FacebookId"];
                    options.ClientSecret = authenticationSecrets["FacebookSecret"];
                    options.AccessDeniedPath = "/Identity/Account/Login";
                })
                .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
                {
                    options.ClientId = authenticationSecrets["GoogleId"];
                    options.ClientSecret = authenticationSecrets["GoogleSecret"];
                    options.AccessDeniedPath = "/Identity/Account/Login";
                })
                .AddMicrosoftAccount(MicrosoftAccountDefaults.AuthenticationScheme, options =>
                {
                    options.ClientId = authenticationSecrets["MicrosoftId"];
                    options.ClientSecret = authenticationSecrets["MicrosoftSecret"];
                    options.AccessDeniedPath = "/Identity/Account/Login";
                });

            builder.Services.AddScoped<IDbInitializer, DbInitializer>();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IEmailSender, EmailSender>();

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IVnPayService, VnPayService>();
            builder.Services.AddSingleton<IVnPayService, VnPayService>();
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

            builder.Services.AddSingleton<TelegramService>(sp =>
            {
                var token = builder.Configuration["TelegramBot:Token"];
                return new TelegramService(token);
            });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            SeedDatabase();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();

            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
            name: "customer",
            pattern: "{area=Customer}/{controller=Payment}/{action=PaymentCallBack}/{id?}");


            app.Run();

            void SeedDatabase()
            {
                using (var scope = app.Services.CreateScope())
                {
                    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
                    try
                    {
                        dbInitializer.Initialize();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error seeding database: {ex.Message}");
                    }
                }
            }
        }
    }
}
