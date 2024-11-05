using B3cBonsai.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.DataAccess.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Facebook; // Thêm namespace cho Facebook
using B3cBonsai.Models;
using B3cBonsai.Utility;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Configuration;
using B3cBonsai.DataAccess.DbInitializer;
using Microsoft.Extensions.Options;

namespace B3cBonsaiWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            // Thiết lập thời gian ngừng nghỉ phiên
            builder.Services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });

            // Thêm DbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString"));
            });
            // Thêm Identity
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Cấu hình cookie
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });

            var facebookOptions = builder.Configuration.GetSection("Authentication:Facebook");
            var googleOptions = builder.Configuration.GetSection("Authentication:Google");

            builder.Services.AddAuthentication()
                .AddCookie() // Đăng ký Cookie authentication trước
                .AddFacebook(options =>
                {
                    options.ClientId = facebookOptions["ClientId"];
                    options.ClientSecret = facebookOptions["ClientSecret"];
                })
                .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
                {
                    options.ClientId = googleOptions["ClientId"];
                    options.ClientSecret = googleOptions["ClientSecret"];
                    options.AccessDeniedPath = $"/Identity/Account/Login";
                });
            // Thêm các dịch vụ cần thiết
            builder.Services.AddScoped<IDbInitializer, DbInitializer>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IEmailSender, EmailSender>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession(); // Đặt session trước xác thực

            SeedDatabase();

            app.UseAuthentication(); // Xác thực người dùng
            app.UseAuthorization(); // Phân quyền

            app.MapRazorPages(); // Map các trang Razor

            // Cấu hình tuyến đường cho Controller
            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

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
                        // Log the exception (you might want to use a logging library)
                        Console.WriteLine($"Error seeding database: {ex.Message}");
                    }
                }
            }
        }
    }
}
