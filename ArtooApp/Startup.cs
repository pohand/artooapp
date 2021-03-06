﻿using System;
using System.IO;
using Artoo.Infrastructure;
using Artoo.IRepositories;
using Artoo.IServices;
using Artoo.Models;
using Artoo.Providers;
using Artoo.Repositories;
using Artoo.Services;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace ArtooApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddUserManager<AppUserManager>(); //http://ericsmasal.com/2018/06/05/capture-tenant-in-asp-net-core-2-0-web-api/


            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                // If the LoginPath isn't set, ASP.NET Core defaults 
                // the path to /Account/Login.
                options.LoginPath = "/Account/Login";
                // If the AccessDeniedPath isn't set, ASP.NET Core defaults 
                // the path to /Account/AccessDenied.
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            services.AddSingleton<IFileProvider>(
                new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

            #region Repository DI
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IMistakeRepository, MistakeRepository>();
            services.AddTransient<IPassionBrandRepository, PassionBrandRepository>();
            services.AddTransient<IFactoryRepository, FactoryRepository>();
            services.AddTransient<IInspectionRepository, InspectionRepository>();
            services.AddTransient<IEmailRepository, EmailRepository>();
            services.AddTransient<IEmailRuleRepository, EmailRuleRepository>();
            services.AddTransient<IDashboardRepository, DashboardRepository>();
            services.AddTransient<IFinalWeekRepository, FinalWeekRepository>();
            services.AddTransient<ITechManagerRepository, TechManagerRepository>();
            services.AddTransient<IMistakeFreeRepository, MistakeFreeRepository>();
            services.AddTransient<IMistakeCategoryRepository, MistakeCategoryRepository>();
            services.AddTransient<IConfigurationRepository, ConfigurationRepository>();
            
            #endregion

            #region Service DI
            services.AddTransient<IInspectionImportService, InspectionImportService>();
            services.AddTransient<IMistakeCategoryService, MistakeCategoryService>();
            #endregion

            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools())); //DinkToPdf
            services.AddScoped(typeof(TenantAttribute));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddMemoryCache();
            services.AddSession();

            services.AddTransient<ITenantProvider, DatabaseTenantProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/AppException");
            }

            app.UseStaticFiles();
            app.UseSession();
            //app.UseIdentity();
            app.UseAuthentication();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
