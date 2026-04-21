using API_HUNT.DataBaseConnection;
using API_HUNT.Models;
using Microsoft.AspNetCore.Http.Features;

namespace API_HUNT
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add<API_HUNT.Models.CustomFilter>();
                options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());
            });

            builder.Services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-CSRF-TOKEN";
            });

            // Form options (mirrors Startup.ConfigureServices)
            builder.Services.Configure<FormOptions>(options => { options.ValueCountLimit = 5000; });

            // Session
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.IsEssential = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
            });

            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddMemoryCache();

            // HttpClient for JIRA controller
            builder.Services.AddHttpClient();

            // Database connection factory — single source of truth for the connection string
            var connectionString = builder.Configuration.GetConnectionString("HuntDb")
                ?? throw new InvalidOperationException("Missing ConnectionStrings:HuntDb in appsettings.json");
            ConnectionDB.SetConnectionString(connectionString);
            builder.Services.AddSingleton<IDbConnectionFactory>(new DbConnectionFactory(connectionString));

            // Repositories
            builder.Services.AddScoped<IActivityLogRepository, ActivityLogRepository>();
            builder.Services.AddScoped<ILoginRepository, LoginRepository>();
            builder.Services.AddScoped<IAdminRepository, AdminRepository>();
            builder.Services.AddScoped<IExceptionRepository, ExceptionRepository>();
            builder.Services.AddScoped<IPartnerRepository, PartnerRepository>();
            builder.Services.AddScoped<IPartnerOnboardingNewRepository, PartnerOnboardingNewRepository>();
            builder.Services.AddScoped<IPartnerApprovalRepository, PartnerApprovalRepository>();
            builder.Services.AddScoped<IPartnerOffboardingRepository, PartnerOffboardingRepository>();
            builder.Services.AddScoped<ISubmitRepository, SubmitRepository>();
            builder.Services.AddScoped<IJiraRepository, JIRARepository>();

            // HomeController registered for injection into JIRACreatorController (Table_Export_IntegratedXL)
            builder.Services.AddTransient<API_HUNT.Controllers.HomeController>();
            builder.Services.AddHttpClient<API_HUNT.Controllers.JIRACreatorController>();

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
            app.UseAuthorization();

            // Default route matches original startupclass.cs intent: Login/Index
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Login}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
