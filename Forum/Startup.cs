using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Forum.Common;
using Forum.Data;
using Forum.Models;
using Forum.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WanderLust.Common;

namespace DevExtremeAspNetCoreApp
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
            // Add framework services.
            services
                .AddControllersWithViews()
                .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null
                    );
            string connectionString = Configuration.GetConnectionString("DefaultConnectionString");
            services.AddDbContext<ApplicationDbContext>(config =>
            {
                config.UseSqlServer(connectionString, b => b.MigrationsAssembly("Forum"));
            });
            #region configure AspNetIdentity
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 1;
                options.Password.RequireNonAlphanumeric = false;
                options.SignIn.RequireConfirmedEmail = true;

            });
            #endregion

            var config = Configuration.GetSection("AppSettings").Get<AppSettings>();
            services.Configure<AppSettings>(Configuration.GetSection("ConnectionStrings"));
            services.Configure<AppSettings>(Configuration.GetSection("appSettings"));
            services.Configure<AppSettings>(Configuration.GetSection("GooglereCaptcha"));
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                
            })
             .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, (options) =>
             {
                 options.Cookie.Name = "_06151813.Cookies";
                 options.LoginPath = "/User/Login";
                 //options.LogoutPath = "/Home";
             });

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            #region External Authentication
            services.AddAuthentication()
              .AddGoogle(options =>
              {
                  options.ClientId = Configuration["Authentication:Google:ClientId"];
                  options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                  options.SignInScheme = IdentityConstants.ExternalScheme;
      })
      .AddFacebook(options =>
      {
          options.AppId = Configuration["Authentication:Facebook:AppId"];
          options.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
          //options.SignInScheme = IdentityConstants.ExternalScheme;
      });
            #endregion

            #region injection
           
            services.AddScoped<ForumDbx>();
            services.AddScoped<UserService>();
            services.AddScoped<HttpContextAccessor>();
            services.AddScoped<MainTopicService>();
            services.AddScoped<TopicInformationService>();
            services.AddScoped<AdminService>();
            services.AddScoped<RegularUserService>();
            services.AddScoped<EmailHelper>();
            services.AddScoped<HostgatorEmailHelper>();
            services.AddServiceExtension();
            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
