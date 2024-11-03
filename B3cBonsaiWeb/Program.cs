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

            // Thêm xác thực với Facebook
            builder.Services.AddAuthentication()
                .AddCookie() // Đăng ký Cookie authentication trước
                .AddFacebook(options =>
                {
                    options.ClientId = "1314586289526497";
                    options.ClientSecret = "8d3ab1c336749f8f8b956b040425e98e";
                })
                .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
                {
                    options.ClientId = "974403152940-7s77p8jicv61gtlcctacmo6l6v59967b.apps.googleusercontent.com";
                    options.ClientSecret = "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";
                });

            // Thêm các dịch vụ cần thiết
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

            app.UseAuthentication(); // Xác thực người dùng
            app.UseAuthorization(); // Phân quyền

            app.MapRazorPages(); // Map các trang Razor

            // Cấu hình tuyến đường cho Controller
            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
