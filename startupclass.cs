using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using API_HUNT.DataBaseConnection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http.Features;

namespace API_HUNT
{
    public class Startup
    {
        public static string connectionstring { get; private set; }
        public static string Emailconnectionstring { get; private set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<FormOptions>(options => { options.ValueCountLimit = 5000; });
            services.AddMemoryCache();
            services.AddSession();
            services.AddMvc();
            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            //    options.CheckConsentNeeded = context => true;
            //    options.MinimumSameSitePolicy = SameSiteMode.None;
            //});
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.IsEssential = true; // make the session cookie Essential
            });


            services.AddMvc();
            services.AddDistributedMemoryCache();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            ConnectionDB connection = new ConnectionDB();
            //Live
            bool isProd = true;
            string strConnection = connection.getConString("1605485", string.Empty, "COLD3whn89PtD+SNyCdvwQ==", isProd);
            //----------------UAT-------------------------------------
            //bool isProd = false; 
            //string strConnection = connection.getConString("1605485", string.Empty, "sAbDgGa7FnfKt4hVBaD7Bg==", isProd);
            //----------------UAT-end------------------------------------


            //connectionstring = strConnection;
            //Emailconnectionstring = strConnectionemail;  
            //string strConnectionemail = VaultAPI_Live.DBConnection.GetDBVault("zMWOpB3jCjLJzCpaF2nWKg==", "2415005", emp, sValues);
            // SMTP connection string removed — use appsettings.json for email configuration
            connectionstring = strConnection;
            //Emailconnectionstring = strConnectionemail;

            if (!isProd)
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseDeveloperExceptionPage();
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //    app.UseHsts();
            //}

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Login}/{action=Index}/{id?}");
            });
        }
    }
}
