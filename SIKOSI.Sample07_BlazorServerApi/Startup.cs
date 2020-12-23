// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 03.12.2020 13:20
// Developer      Roman Jahn
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SIKOSI.Crypto;
using SIKOSI.Crypto.Interfaces;
using SIKOSI.Sample07_EncryptedChat.Helper;
using SIKOSI.Sample07_EncryptedChat.Hubs;
using SIKOSI.SampleDatabase02.Context;
using SIKOSI.SampleDatabase02.Entities;
using SIKOSI.Services.Auth;
using SIKOSI.Services.Auth.Interfaces;

namespace SIKOSI.Sample07_EncryptedChat
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
        public void ConfigureServices(IServiceCollection services)
        {
            // configure jwt authentication
            var key = Encoding.ASCII.GetBytes("TheS3cr3Tt0kenItIs!REPLACE!!:-)");

            services.AddDbContext<DataContext, SqliteDataContext>();

            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            var appSettings = appSettingsSection.Get<AppSettings>();

            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    // Configure the Authority to the expected value for your authentication provider
                    // This ensures the token is appropriately validated
                    //options.Authority = /* TODO: Insert Authority URL here */;

                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                                                  {
                                                      ValidateIssuerSigningKey = true,
                                                      ValidateLifetime = true,
                                                      IssuerSigningKey = new SymmetricSecurityKey(key),
                                                      ValidateIssuer = false,
                                                      ValidateAudience = false,
                                                      ClockSkew = TimeSpan.Zero
                                                  };
                    // We have to hook the OnMessageReceived event in order to
                    // allow the JWT authentication handler to read the access
                    // token from the query string when a WebSocket or 
                    // Server-Sent Events request comes in.

                    // Sending the access token in the query string is required due to
                    // a limitation in Browser APIs. We restrict it to only calls to the
                    // SignalR hub in this code.
                    // See https://docs.microsoft.com/aspnet/core/signalr/security#access-token-logging
                    // for more information about security considerations when using
                    // the query string to transmit the access token.
                    x.Events = new JwtBearerEvents
                               {
                                   OnMessageReceived = context =>
                                   {
                                       var accessToken = context.Request.Query["access_token"];

                                       // If the request is for our hub...
                                       var path = context.HttpContext.Request.Path;
                                       //if (!string.IsNullOrEmpty(accessToken) &&
                                       //    (path.StartsWithSegments("/hubs/chat")))
                                       // Read the token out of the query string
                                       if (!string.IsNullOrEmpty(accessToken))
                                           context.Token = accessToken;
                                       return Task.CompletedTask;
                                   }
                               };
                });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);


            services.AddSignalR(o =>
            {
                //increase Maximumsize for sending images/files over signalR (default is 32768 i.e. 32 KB)
                o.MaximumReceiveMessageSize = 32768000;
            });
            //services.AddMvc().AddNewtonsoftJson();

            //services.AddControllers();
            services.AddControllersWithViews();

            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] {"application/octet-stream"});
            });

            //TODO: RICHTIG einstellen!
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.AllowAnyHeader()
                            //.WithOrigins("https://*:44311", "https://*:44356")
                            .AllowAnyOrigin();
                        //.AllowCredentials();
                    });
            });

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
                c.SwaggerDoc("v2", new OpenApiInfo
                                   {
                                       Contact = new OpenApiContact
                                                 {
                                                     Email = "biss@fotec.at",
                                                     Name = "FOTEC - Forschungs- und Technologietransfer GmbH",
                                                     Url = new Uri("https://www.fotec.at")
                                                 },
                                       Description = "API SIKOSI Sample7",
                                       License = new OpenApiLicense
                                                 {
                                                     Name = "GPLV3"
                                                 },
                                       Title = "SIKOSI Sample07 EncryptedChat",
                                       Version = "1.0"
                                   });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, "Common", xmlFile);

                if (File.Exists(xmlPath))
                    c.IncludeXmlComments(xmlPath);
            });

            // DI for user service
            services.AddScoped<IUserFileService, UserFileService<TblUser, DataContext>>();

            //services.AddSingleton<IUserService, UserService<TblUser, DataContext>>();
            //services.AddSingleton<DataContext>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // use NaCl for crossplatform encryption
            services.AddSingleton<ISecureEncryption, SecureEncryptionDhCrossPlatform>();
            //services.AddSingleton<ISecureEncryption, SecureEncryptionDh>(sp => new SecureEncryptionDh(new EcdhNaCl()));

            services.AddLogging();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors();
            app.UseWebSockets();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/chathub");
            });

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v2/swagger.json", "Rest API"); });
        }
    }
}