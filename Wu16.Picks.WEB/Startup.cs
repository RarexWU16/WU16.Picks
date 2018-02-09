using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Wu16.Picks.WEB.DataAccess;
using Wu16.Picks.WEB.Repositories;
using static Microsoft.AspNetCore.WebSockets.Internal.Constants;

namespace Wu16.Picks.WEB
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
            services.AddDbContext<ApplicationDbContext>(options => 
                options.UseSqlServer(Configuration.GetConnectionString("ApplicationDbContext")));

            services.AddTransient<CategoryRepository>();
            services.AddTransient<ImageRepository>();

            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = Configuration.GetConnectionString("redis");
                options.InstanceName = "PICKS-REDIS";
            });

            services.AddSession(opt =>
            {
                opt.Cookie.Name = "picks.io";
                opt.IdleTimeout = TimeSpan.FromMinutes(60);
            });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
                app.UseStaticFiles();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseStaticFiles(new StaticFileOptions() {
                    OnPrepareResponse = x =>
                    {
                        x.Context.Response.Headers["Cache-Control"] = "public, max-age=31557600";
                    }
                });
            }

            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
