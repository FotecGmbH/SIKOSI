// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 11.05.2020 14:40
// Developer      Manuel Fasching
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using System;
using System.IO;
using System.Reflection;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Sample0_Basic.Entities;
using SIKOSI.Sample01_Auth.Helpers;
using SIKOSI.SampleDatabase01.Context;
using SIKOSI.Services.Auth;
using SIKOSI.Services.Auth.Interfaces;

namespace SIKOSI.Sample01_Auth
{
    /// <summary>
    /// Startup
    /// </summary>
    public class Startup
    {
        /// <summary>
        ///     Welche Swagger Version soll verwendet werden (v1, v2, v3)
        /// </summary>
        private const string SwaggerVersion = "v2";

        /// <summary>
        /// Config
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// WebHostEnvironment
        /// </summary>
        private readonly IWebHostEnvironment _env;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="env">Environment</param>
        /// <param name="configuration">Configuration</param>
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">Services</param>
        public void ConfigureServices(IServiceCollection services)
        {
            //TODO: Health-Check einbauen
            //services.AddHealthChecks().AddDbContextCheck();

            //TODO: Switch dev server
            // use sql server db in production and sqlite db in development
            //if (_env.IsProduction())
            //    services.AddDbContext<DataContext>();
            //else

            services.AddDbContext<DataContext, SqliteDataContext>();
            
            //TODO: InMemoryDatabase
            //services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("abc"));

            //services.AddCors(); TODO: Secure or not?

            var appSettingsSection = _configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            var appSettings = appSettingsSection.Get<AppSettings>();

            // DI for user service
            services.AddScoped<IUserService, UserService<TblUser, DataContext>>();

            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                                                  {
                                                      ValidateIssuerSigningKey = true,
                                                      ValidateLifetime = true,
                                                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appSettings.Secret)),
                                                      ValidateIssuer = false,
                                                      ValidateAudience = false,
                                                      ClockSkew = TimeSpan.Zero
                                                  };
                });

            services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.IgnoreNullValues = true);

            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                                                  {
                                                      Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer ATDK29JKLME')",
                                                      Name = "Authorization",
                                                      In = ParameterLocation.Header,
                                                      Type = SecuritySchemeType.ApiKey,
                                                      Scheme = "Bearer"
                                                  });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                                         {
                                             {
                                                 new OpenApiSecurityScheme
                                                 {
                                                     Reference = new OpenApiReference
                                                                 {
                                                                     Type = ReferenceType.SecurityScheme,
                                                                     Id = "Bearer"
                                                                 }
                                                 },
                                                 Array.Empty<string>()
                                             }
                                         });
                c.SwaggerDoc(SwaggerVersion, new OpenApiInfo
                                             {
                                                 Contact = new OpenApiContact
                                                           {
                                                               Email = "biss@fotec.at",
                                                               Name = "FOTEC - Forschungs- und Technologietransfer GmbH",
                                                               Url = new Uri("https://www.fotec.at")
                                                           },
                                                 Description = "API SIKOSI Sample0_Auth",
                                                 License = new OpenApiLicense
                                                           {
                                                               Name = "GPLV3"
                                                           },
                                                 Title = "SIKOSI Sample01 Auth",
                                                 Version = "1.0"
                                             });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, "Common", xmlFile);

                if (File.Exists(xmlPath))
                    c.IncludeXmlComments(xmlPath);
            });


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

        /// <summary>
        /// his method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">App</param>
        /// <param name="env">Environment</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint($"/swagger/{SwaggerVersion}/swagger.json", "Rest API"); });
        }
    }
}