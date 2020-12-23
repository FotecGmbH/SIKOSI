// (C) 2019 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created:        03.12.2020 16:34
// Developer:      Gregor Faiman
// Project         SIKOSI
//
// Released under GPL-3.0-only

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace SIKOSI.SampleDatabase01.Context
{
    public class SqliteDataContext : DataContext
    {
        public SqliteDataContext(IConfiguration configuration) : base(configuration) { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sqlite database
            options.UseSqlite(Configuration.GetConnectionString("WebApiDatabase"));
        }
    }
}
