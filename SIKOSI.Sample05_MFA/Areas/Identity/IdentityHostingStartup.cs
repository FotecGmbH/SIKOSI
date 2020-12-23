// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 30.11.2020 08:19
// Developer      Benjamin Moser
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using System;
using MFA_QR_CODE.Areas.Identity;
using MFA_QR_CODE.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(IdentityHostingStartup))]

namespace MFA_QR_CODE.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        #region Interface Implementations

        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                services.AddDbContext<MFA_QR_CODEContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("MFA_QR_CODEContextConnection")));

                services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<MFA_QR_CODEContext>();
            });
        }

        #endregion
    }
}