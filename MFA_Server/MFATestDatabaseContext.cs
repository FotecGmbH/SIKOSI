using MFA_Server.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFA_Server
{
    public class MFATestDatabaseContext : DbContext
    {
        public MFATestDatabaseContext(DbContextOptions<MFATestDatabaseContext> options) : base(options)
        {
        }

        public DbSet<TblUser> Users
        {
            get;
            set;
        }
    }
}
