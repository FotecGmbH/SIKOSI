// (C) 2019 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created:        03.12.2020 16:25
// Developer:      Gregor Faiman
// Project         SIKOSI
//
// Released under GPL-3.0-only

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace SIKOSI.SampleDatabase03.Context
{
    public class SRPSqliteDataContext : SRPDataContext
    {
        public SRPSqliteDataContext(IConfiguration configuration) : base(configuration) { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sqlite database
            options.UseSqlite(Configuration.GetConnectionString("WebApiDatabase"));
        }
    }
}
