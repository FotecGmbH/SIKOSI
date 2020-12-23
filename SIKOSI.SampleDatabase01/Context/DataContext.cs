// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 27.11.2020 10:10
// Developer      Manuel Fasching
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sample0_Basic.Entities;
using SIKOSI.Services.DB.Interfaces;

namespace SIKOSI.SampleDatabase01.Context
{
    public class DataContext : DbContext, IUserDataContext<TblUser>
    {
        protected readonly IConfiguration Configuration;

        public DataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(Configuration.GetConnectionString("SIKOSI_DB_S0"));
        }

        #region Interface Implementations

        public DbSet<TblUser> Users { get; set; }

        #endregion

        //public DbSet<TblRefreshToken> RefreshTokens { get; set; }
    }
}