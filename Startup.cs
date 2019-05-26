using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HW_06.Data;
using HW_06.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HW_06.Models;

namespace HW_06
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

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddDefaultTokenProviders()
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<ApplicationDbContext>();


            services.AddAuthorization(options =>
            {
                options.AddPolicy(MyIdentityDataService.Policy_Add, policy => policy.RequireRole(MyIdentityDataService.AdminRoleName));
                options.AddPolicy(MyIdentityDataService.Policy_Edit, policy => policy.RequireRole(MyIdentityDataService.AdminRoleName, MyIdentityDataService.EditorRoleName));
                options.AddPolicy(MyIdentityDataService.Policy_Comment, policy => policy.RequireRole(MyIdentityDataService.AdminRoleName, MyIdentityDataService.EditorRoleName, MyIdentityDataService.AuthenticatedRoleName));
                options.AddPolicy(MyIdentityDataService.Policy_Delete, policy => policy.RequireRole(MyIdentityDataService.AdminRoleName));
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IConfiguration configuration, UserManager<IdentityUser> user, RoleManager<IdentityRole> role)
        {
            var cmsUrlConstraint = new CmsUrlConstraint(configuration);
            MyIdentityDataService.SeedData(user, role);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Quote}/{id?}"
                    );

                routes.MapRoute(
                    name: "CmsRoute", 
                    template: "{*permalink}",
                    defaults: new { controller = "Content", action = "Index" },
                    constraints: new { permalink = cmsUrlConstraint }
                    );
            });
        }
    }
}
