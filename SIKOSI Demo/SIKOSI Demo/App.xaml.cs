// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 09.11.2020 07:43
// Developer      Roman Jahn
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using SIKOSI.Crypto;
using SIKOSI.Crypto.Interfaces;
using SIKOSI.Exchange.Model;
using SIKOSI.Sample02.ViewModels;
using SIKOSI.Sample02.Views;
using SIKOSI.SecureServices;
using Xamarin.Forms;

namespace SIKOSI.Sample02
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var services = new ServiceCollection();

            // register services
            services.AddScoped(sp => new HttpClient {BaseAddress = new Uri("http://10.0.2.2:55311/")}); // "https://10.0.0.1:44311/" "http://10.0.2.2:55311/" "https://localhost:44311/"
            services.AddScoped<AuthUserModel>();
            services.AddScoped<EncryptedLoginService>();
            services.AddScoped<EncryptedRegistrationService>();
            services.AddScoped<ISecureSymmetricEncryption, SecureEncryptionAesCrossPlatform>();
            services.AddScoped<ISecureEncryption, SecureEncryptionDhCrossPlatform>();
            services.AddScoped<ViewModelChatOverview>();
            services.AddScoped<ViewModelLogin>();
            services.AddScoped<ViewModelRegistration>();
            services.AddScoped<ViewModelCrypt>();

            //build service provider
            ServiceProvider = services.BuildServiceProvider();

            MainPage = new NavigationPage(new ViewLogin());
        }

        #region Properties

        public static IServiceProvider ServiceProvider { get; private set; }

        #endregion

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}