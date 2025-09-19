using System.Configuration;
using B3cBonsai.DataAccess.Data;
using B3cBonsai.DataAccess.DbInitializer;
using B3cBonsai.DataAccess.Repository;
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using B3cBonsai.Utility;
using B3cBonsai.Utility.Helper;
using B3cBonsaiWeb.Attributes;
using B3cBonsaiWeb.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

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
            /*options =>
            {
                options.Filters.Add<CheckUserStatusAttribute>();
            }*/
            builder.Services.AddSession(options =>
            {
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

            // Tải secrets từ AWS Secrets Manager (TẠM THỜI VÔ HIỆU HÓA)
            //var secretsService = new SecretsManagerService(builder.Configuration);
            //var authenticationSecrets = await secretsService.GetSecretAsync(); // Sử dụng await ở đây

            builder.Services.AddAuthentication()
                .AddCookie()
                /*.AddFacebook(FacebookDefaults.AuthenticationScheme, options =>
                {
                    // Cấu hình này sẽ đọc từ appsettings.json
                    options.ClientId = builder.Configuration["Authentication:Facebook:ClientId"];
                    options.ClientSecret = builder.Configuration["Authentication:Facebook:ClientSecret"];
                    options.AccessDeniedPath = "/Identity/Account/Login";
                })
                .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
                {
                    // Cấu hình này sẽ đọc từ appsettings.json
                    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
                    options.AccessDeniedPath = "/Identity/Account/Login";
                })
                .AddMicrosoftAccount(MicrosoftAccountDefaults.AuthenticationScheme, options =>
                {
                    // Cấu hình này sẽ đọc từ appsettings.json
                    options.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"];
                    options.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"];
                    options.AccessDeniedPath = "/Identity/Account/Login";
                })*/;

            builder.Services.AddScoped<IDbInitializer, DbInitializer>();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IEmailSender, EmailSender>();

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IVnPayService, VnPayService>();

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
