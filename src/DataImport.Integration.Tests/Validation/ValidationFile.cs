﻿using System;
using System.Collections.Generic;
using Domain.Core.Trading.Orders;

namespace DataImport.Integration.Tests.Validation
{
    public class ValidationFile
    {
        public ValidationFile(string path, bool success, int successfulRows, IReadOnlyCollection<Func<Order, bool>> rows)
        {
            Path = path;
            Success = success;
            SuccessfulRows = successfulRows;
            RowAssertions = rows ?? new Func<Order, bool>[0];
        }

        public string Path { get; }
        public bool Success { get; }
        public int SuccessfulRows { get; }
        public IReadOnlyCollection<Func<Order, bool>> RowAssertions { get;  }
    }
}
