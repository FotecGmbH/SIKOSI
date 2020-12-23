// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 07.12.2020 21:19
// Developer      Manuel Fasching
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SIKOSI.SampleDatabase02.Entities;
using SIKOSI.Services.DB.Interfaces;

namespace SIKOSI.SampleDatabase02.Context
{
    public class DataContext : DbContext, IUserFilesDataContext<TblUser>
    {
        protected readonly IConfiguration Configuration;

        public DataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #region Properties

        public DbSet<TblMessage> Messages { get; set; }

        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(Configuration.GetConnectionString("SIKOSI_DB_S0"));
        }

        #region Interface Implementations

        public DbSet<TblUser> Users { get; set; }
        public DbSet<TblUser> UserFiles { get; set; }

        #endregion
    }
}