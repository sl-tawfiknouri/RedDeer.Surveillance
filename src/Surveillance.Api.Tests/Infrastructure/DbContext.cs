using Microsoft.EntityFrameworkCore;
using Surveillance.Api.DataAccess.DbContexts;
using Surveillance.Api.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Api.Tests.Infrastructure
{
    public class DbContext : GraphQlDbContext
    {
        public DbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<RuleBreach> RuleBreaches => _ruleBreach;
    }
}
