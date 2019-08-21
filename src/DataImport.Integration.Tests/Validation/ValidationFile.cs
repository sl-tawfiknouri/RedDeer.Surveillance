namespace DataImport.Integration.Tests.Validation
{
    using System;
    using System.Collections.Generic;

    using Domain.Core.Trading.Orders;

    public class ValidationFile
    {
        public ValidationFile(
            string path,
            bool success,
            int successfulRows,
            IReadOnlyCollection<Func<Order, bool>> rows)
        {
            this.Path = path;
            this.Success = success;
            this.SuccessfulRows = successfulRows;
            this.RowAssertions = rows ?? new Func<Order, bool>[0];
        }

        public string Path { get; }

        public IReadOnlyCollection<Func<Order, bool>> RowAssertions { get; }

        public bool Success { get; }

        public int SuccessfulRows { get; }
    }
}