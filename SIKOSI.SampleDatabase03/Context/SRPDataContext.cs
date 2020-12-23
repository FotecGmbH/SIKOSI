// (C) 2019 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created:        03.12.2020 16:21
// Developer:      Gregor Faiman
// Project         SIKOSI
//
// Released under GPL-3.0-only

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SIKOSI.SampleDatabase03.Entities;

namespace SIKOSI.SampleDatabase03.Context
{
    public class SRPDataContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public SRPDataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(Configuration.GetConnectionString("SIKOSI_DB_S0"));
        }

        /// <summary>
        /// The users table.
        /// </summary>
        public DbSet<User> Users
        {
            get;
            set;
        }
    }
}
