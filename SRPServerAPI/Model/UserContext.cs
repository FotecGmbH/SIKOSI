// (C) 2019 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created:        16.09.2020 14:18
// Developer:      Gregor Faiman
// Project         SIKOSI
//
// Released under GPL-3.0-only

using Microsoft.EntityFrameworkCore;

namespace SRPServerAPI.Model
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {
        }

        public DbSet<ApplicationUser> Users
        {
            get;
            set;
        }
    }
}
