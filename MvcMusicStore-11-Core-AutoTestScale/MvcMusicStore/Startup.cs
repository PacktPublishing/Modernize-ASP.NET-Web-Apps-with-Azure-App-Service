using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MvcMusicStore.Models;
using Newtonsoft.Json;
using Strathweb.AspNetCore.AzureBlobFileProvider;


namespace MvcMusicStore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            services.Configure<CookiePolicyOptions>(options =>
            {
                //options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddSession(actions =>
                 {
                     actions.IdleTimeout = TimeSpan.FromMinutes(20);
                     actions.Cookie.HttpOnly = true;
                 });
            services.AddDistributedRedisCache(o =>
            {
                o.Configuration = Configuration.GetConnectionString("RedisCache");
            });
            services.AddMvc().AddJsonOptions(opt =>
            {
                opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                opt.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            });

            services.AddHttpContextAccessor();

            services.AddAuthentication(AzureADB2CDefaults.AuthenticationScheme)
                  .AddAzureADB2C(options => Configuration.Bind("AzureAdB2C", options)).AddCookie();

            var blobOptions = new AzureBlobOptions
            {
                ConnectionString = Configuration.GetConnectionString("AzureStorageStaticFiles"),
                DocumentContainer = "wwwroot"
            };
            var azureBlobFileProvider = new AzureBlobFileProvider(blobOptions);
            services.AddSingleton(azureBlobFileProvider);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            //app.UseStaticFiles();
            var blobFileProvider = app.ApplicationServices.GetRequiredService<AzureBlobFileProvider>();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = blobFileProvider,
                RequestPath = ""
            });
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseSession();
            app.UseMvc(routes =>    
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
