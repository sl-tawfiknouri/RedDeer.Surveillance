using System;
using System.Collections.Generic;
using System.Linq;
using DataImport.Disk_IO.EtlFile.Interfaces;
using FluentValidation.Results;
using SharedKernel.Files.Orders;

namespace DataImport.Disk_IO.EtlFile
{
    public class EtlErrorRecord : IEtlErrorRecord
    {
        public EtlErrorRecord(
            OrderFileContract record,
            IList<ValidationFailure> failures)
        {
            Record = record;
            Failures = failures ?? new ValidationFailure[0];
        }

        private OrderFileContract Record { get; }
        private IList<ValidationFailure> Failures { get; }

        public override string ToString()
        {
            var recordData = $"Order id {Record.OrderId}";

            var failureMessages =
                Failures?.Select(FailureMessage).ToList()
                ?? new List<string>();

            var projectedFailureMessages =
                failureMessages.Any() ?
                failureMessages.Aggregate(string.Empty, (x, y) => $"{x} {y} {Environment.NewLine}")
                : string.Empty;

            return $"{recordData} {Environment.NewLine} {projectedFailureMessages} {Environment.NewLine}";
        }

        private string FailureMessage(ValidationFailure failure)
        {
            if (failure == null)
            {
                return string.Empty;
            }

            return $"{failure.PropertyName}. {failure.ErrorMessage}";
        }
    }
}
