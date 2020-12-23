using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using SIKOSI.Sample02_SRP.Helpers;
using SIKOSI.SampleDatabase01.Context;
using SIKOSI.Services.Auth;
using SIKOSI.Services.Auth.Interfaces;
using SIKOSI.Sample02_SRP.Services;
using SIKOSI.SampleDatabase03.Context;

namespace SIKOSI.Sample02_SRP
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        /// <summary>
        ///     Welche Swagger Version soll verwendet werden (v1, v2, v3)
        /// </summary>
        private const string SwaggerVersion = "v2";

        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //TODO: Health-Check einbauen
            //services.AddHealthChecks().AddDbContextCheck();

            //TODO: Switch dev server
            // use sql server db in production and sqlite db in development
            //if (_env.IsProduction())
            //    services.AddDbContext<DataContext>();
            //else
            services.AddDbContext<SRPDataContext, SRPSqliteDataContext>();

            //TODO: InMemoryDatabase
            //services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("abc"));

            //services.AddCors(); TODO: Secure or not?

            var appSettingsSection = _configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.IgnoreNullValues = true);
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(SwaggerVersion, new OpenApiInfo(){Contact = new OpenApiContact(){Email = "biss@fotec.at", Name = "FOTEC"},Description = "SIKOSI WebApi Sample0_Auth"});
                //c.AddSecurityDefinition("Bearer", new ApiKeyScheme() { In = "header", Description = "Token hier eingeben (Bearer + Token) bzw. reinkopieren", Name = "Authorization", Type = "apiKey" });
                //c.SwaggerDoc(SwaggerVersion, new Info
                //{
                //    Title = ResWebCommon.SwaggerTitle,
                //    Version = SwaggerVersion,
                //    Description = ResWebCommon.SwaggerDescription + additionalInformation,
                //    Contact = new Contact
                //    {
                //        Name = ResWebCommon.SwaggerContactName,
                //        Email = ResWebCommon.SwaggerContactEMail,
                //        Url = ResWebCommon.SwaggerContactUrl
                //    }
                //});

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, "Common", xmlFile);

                if (File.Exists(xmlPath))
                    c.IncludeXmlComments(xmlPath);
            });

            // DI for user service
            services.AddScoped<ISRPProtocolService, SRPProtocolService>();
            services.AddSingleton<DataCache>();

            //TODO: Antiforgery

            //services.AddAntiforgery(options => 
            //{
            //    // Set Cookie properties using CookieBuilder properties†.
            //    options.FormFieldName = "AntiforgeryFieldname";
            //    options.HeaderName = "X-CSRF-TOKEN-HEADERNAME";
            //    options.SuppressXFrameOptionsHeader = false;
            //});

            //Sample0_Basic.xml

            //Configure Automapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //TODO: Für JS Client
            //app.UseDefaultFiles();
            //app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseRouting();

            //TODO: Authentication aktivieren!
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{SwaggerVersion}/swagger.json", "Rest API");
            });
        }
    }
}
