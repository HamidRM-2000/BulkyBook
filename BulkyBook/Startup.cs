using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BulkyBook.DataLayer.Context;
using BulkyBook.DataLayer.Services.UnitOfWork;
using Microsoft.AspNetCore.Identity.UI.Services;
using BulkyBook.Utility;

namespace BulkyBook
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
         services.AddDbContext<BulkyBook_DBEntities>(options =>
             options.UseSqlServer(
                 Configuration.GetConnectionString("DefaultConnection")));
         services.AddIdentity<IdentityUser, IdentityRole>(options =>
         {
            options.Password.RequiredUniqueChars = 0;
            options.Password.RequireDigit = false;
            options.Password.RequireUppercase = false;
         }).AddDefaultTokenProviders()
             .AddEntityFrameworkStores<BulkyBook_DBEntities>();
         services.AddSingleton<IEmailSender, EmailSender>();
         services.AddScoped<IUnitOfWork, UnitOfWork>();

         services.AddControllers().AddNewtonsoftJson(options =>
         options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
         );
         services.ConfigureApplicationCookie(options =>
         {
            options.LoginPath = $"/Identity/Account/Login";
            options.LogoutPath = $"/Identity/Account/Logout";
            options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
         });

         services.AddAuthentication()
          .AddGoogle(options =>
          {
             IConfigurationSection googleAuthNSection =
                     Configuration.GetSection("Authentication:Google");

             options.ClientId = googleAuthNSection["ClientId"];
             options.ClientSecret = googleAuthNSection["ClientSecret"];
          });
         services.AddAuthentication().AddFacebook(options =>
         {
            options.AppId = Configuration["Authentication:Facebook:AppId"];
            options.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
            options.AccessDeniedPath = "/AccessDeniedPathInfo";
         });
         services.AddSession(options =>
         {
            options.IdleTimeout = TimeSpan.FromMinutes(60);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
         });
         services.AddAuthorization();
         services.AddControllersWithViews();
         services.AddRazorPages().AddRazorRuntimeCompilation();
      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
      {
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

         app.UseRouting();

         app.UseAuthentication();
         app.UseAuthorization();
         app.UseSession();
         app.UseEndpoints(endpoints =>
         {
            endpoints.MapControllerRoute(
                   name: "defalut",
                   pattern: "{area?}/{controller=Home}/{action=Index}/{id?}"
                   );
            endpoints.MapRazorPages();
         });
      }
   }
}
