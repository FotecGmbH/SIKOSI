// (C) 2019 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created:        16.09.2020 14:18
// Developer:      Gregor Faiman
// Project         SIKOSI
//
// Released under GPL-3.0-only

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using MFA_QR_CODE.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace MFA_QR_CODE
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
            services.AddMvc();

            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddAuthentication()
                // App secret und App id sollten in einer realen Applikation sicher verwahrt werden, und nicht hardcoded im Code vorhanden sein.
                .AddFacebook(options => 
                    { 
                        var faceBookSecrets =
                            Configuration.GetSection("Authentication:Facebook");
                        
                        options.AccessDeniedPath = "/SignIn/AccessDenied";
                        options.AppId = faceBookSecrets["ClientId"];
                        options.AppSecret = faceBookSecrets["ClientSecret"];

                    })
                .AddMicrosoftAccount(options =>
                {
                    var microsoftSecrets =
                        Configuration.GetSection("Authentication:Microsoft");

                    options.ClientId = microsoftSecrets["ClientId"];
                    options.ClientSecret = microsoftSecrets["ClientSecret"];
                })
                .AddGoogle(options =>
                {
                    var googleSecrets =
                        Configuration.GetSection("Authentication:Google");

                    options.ClientId = googleSecrets["ClientId"];
                    options.ClientSecret = googleSecrets["ClientSecret"];
                    options.SignInScheme = IdentityConstants.ExternalScheme;
                });

            // Für diese Test applikation absichtlich die Anforderungen für ein Passwort massiv heruntergesetzt.
            services.Configure<IdentityOptions>(config =>
            {
                config.Password.RequireDigit = false;
                config.Password.RequireLowercase = false;
                config.Password.RequireUppercase = false;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequiredUniqueChars = 1;
                config.Password.RequiredLength = 3;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            services.AddRazorPages();
            services.AddDbContext<MFA_QR_CODEContext>(options => options.UseSqlServer(Configuration.GetConnectionString("MFA_QR_CODEContextConnection")));
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}
