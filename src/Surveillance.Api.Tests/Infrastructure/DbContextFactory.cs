﻿using Microsoft.EntityFrameworkCore;
using Surveillance.Api.DataAccess.Abstractions.DbContexts;
using Surveillance.Api.DataAccess.Abstractions.DbContexts.Factory;
using Surveillance.Api.Tests.Tests;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Api.Tests.Infrastructure
{
    public class DbContextFactory : IGraphQlDbContextFactory
    {
        public IGraphQlDbContext Build()
        {
            var optionBuilders = new DbContextOptionsBuilder<DbContext>();
            optionBuilders.UseInMemoryDatabase("inMemoryDB", (inMemoryDbContextOptionsBuilder) => { });
            var context = new DbContext(optionBuilders.Options);
            return context;
        }
    }
}