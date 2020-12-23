// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 09.11.2020 12:19
// Developer      Roman Jahn
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using System;
using System.Net.Http;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SIKOSI.Crypto;
using SIKOSI.Crypto.Interfaces;
using SIKOSI.Exchange.Model;
using SIKOSI.Sample02.ViewModels;
using SIKOSI.SecureServices;

namespace SIKOSI.Sample01_WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #region Properties

        public IConfiguration Configuration { get; }

        #endregion

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var httpBaseAddress = Configuration.GetSection("ServerBaseAddress").Value;

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddScoped(sp => new HttpClient {BaseAddress = new Uri(httpBaseAddress)});
            services.AddScoped<AuthUserModel>();
            services.AddScoped<EncryptedLoginService>();
            services.AddScoped<EncryptedRegistrationService>();
            services.AddScoped<ISecureSymmetricEncryption, SecureEncryptionAesCrossPlatform>();
            services.AddScoped<ISecureEncryption, SecureEncryptionDhCrossPlatform>();
            //services.AddScoped<ISecureEncryption, SecureEncryptionDh>(sp => new SecureEncryptionDh(new EcdhNaCl()));
            services.AddScoped<ViewModelChatOverview>();
            services.AddScoped<ViewModelLogin>();
            services.AddScoped<ViewModelRegistration>();

            //TODO Add Services

            services.AddBlazorise(options => { options.ChangeTextOnKeyPress = true; }).AddBootstrapProviders().AddFontAwesomeIcons();
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });

            app.ApplicationServices.UseBootstrapProviders().UseFontAwesomeIcons();
        }
    }
}