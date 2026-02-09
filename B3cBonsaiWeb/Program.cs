using System.Configuration;
using B3cBonsai.DataAccess.Data;
using B3cBonsai.DataAccess.DbInitializer;
using B3cBonsai.DataAccess.Repository;
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Utility;
using B3cBonsai.Utility.Helper;
using B3cBonsai.Utility.Services;
using B3cBonsai.Utility.Services.Email;
using B3cBonsai.Utility.Services.Email.Abstractions;
using B3cBonsai.Utility.Services.AI;
using B3cBonsaiWeb.Attributes;
using B3cBonsaiWeb.Services.Notification;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.DataProtection;
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

            // Config Data Protection to persist keys (fixes warning on restart)
            builder.Services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "DataProtection-Keys")))
                .SetApplicationName("B3cBonsai");

            // Add DbContext with provider switching
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                if (builder.Configuration.GetValue<bool>("UsePostgreSql"))
                {
                    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreConnectString"));
                }
                else
                {
                    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString"));
                }
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
            builder.Services.AddScoped<IEmailTemplateReader, FileSystemEmailTemplateReader>();
            builder.Services.AddScoped<ISmtpClientFactory, SmtpClientFactory>();

            if (builder.Configuration.GetValue<bool>("UseCloudinaryStorage"))
            {
            }
            else
            {
                builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
            }

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IVnPayService, VnPayService>();
            builder.Services.AddScoped<IAIService, GeminiAIService>();
            builder.Services.AddScoped<NotificationService>();
            builder.Services.AddScoped<IHtmlSanitizerService, HtmlSanitizerService>();

            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

            builder.Services.AddSingleton<TelegramService>(sp =>
            {
                var token = builder.Configuration["TelegramBot:Token"];
                var logger = sp.GetRequiredService<ILogger<TelegramService>>();

                if (string.IsNullOrEmpty(token))
                {
                     // Return service with dummy token and logger
                     return new TelegramService("dummy_token_to_prevent_crash_check_appsettings", logger);
                }
                return new TelegramService(token, logger);
            });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            // app.UseHttpsRedirection();
            
            // Set up custom content types - associating file extension to MIME type
            var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
            // Add or overwrite 3D model mappings
            provider.Mappings[".glb"] = "model/gltf-binary";
            provider.Mappings[".gltf"] = "model/gltf+json";

            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider
            });

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
