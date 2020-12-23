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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MFA_QR_CODE.Data
{
    public class MFA_QR_CODEContext : IdentityDbContext<IdentityUser>
    {
        public MFA_QR_CODEContext(DbContextOptions<MFA_QR_CODEContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}