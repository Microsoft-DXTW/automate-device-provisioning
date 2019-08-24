﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace hub
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
            #region Workaround client CORS error
            //https://docs.microsoft.com/zh-tw/aspnet/core/signalr/javascript-client?view=aspnetcore-2.2
            services.AddCors(options => options.AddPolicy("CorsPolicy", 
            builder => 
            {
                builder.AllowAnyMethod().AllowAnyHeader()
                       .AllowAnyOrigin()
                       //.WithOrigins("http://localhost:5001")
                       .AllowCredentials();
            }));
            #endregion

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            var connString = Configuration.GetSection("SignalRService").Value;
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSignalR().AddAzureSignalR(connString);
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            #region Workaround client CORS issue
            // app.UseCors(builder =>
            // {
            //     builder.WithOrigins("https://example.com")
            //         .AllowAnyHeader()
            //         .AllowAnyMethod()
            //         //.WithMethods("GET", "POST")
            //         .AllowCredentials();
            // });     
            #endregion
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseAzureSignalR(routes =>{
                routes.MapHub<Hubs.IoTEventHub>("/deviceEvents");
            });
        }
    }
}
