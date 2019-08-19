namespace DataImport.Disk_IO.EtlFile
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using DataImport.Disk_IO.EtlFile.Interfaces;

    using FluentValidation.Results;

    using SharedKernel.Files.Orders;

    public class EtlUploadErrorStore : IEtlUploadErrorStore
    {
        private IList<IEtlErrorRecord> _etlErrors = new List<IEtlErrorRecord>();

        public void Add(OrderFileContract record, IList<ValidationFailure> validationFailures)
        {
            if (record == null) return;

            var etlRecord = new EtlErrorRecord(record, validationFailures);
            this._etlErrors.Add(etlRecord);
        }

        public void Clear()
        {
            this._etlErrors = new List<IEtlErrorRecord>();
        }

        public string SerialisedErrors()
        {
            return this._etlErrors.Select(i => i.ToString()).Aggregate(
                string.Empty,
                (x, i) => $"{x} {i} {Environment.NewLine}");
        }

        public IEnumerable<string> SerialisedErrors(int segmentLimit)
        {
            var listCnt = this._etlErrors.Count;
            var iterations = 0;

            while (listCnt > 0)
            {
                listCnt -= segmentLimit;
                var errors = this._etlErrors.Skip(iterations * segmentLimit).Take(segmentLimit);
                iterations++;

                yield return errors.Select(i => i.ToString()).Aggregate(
                    string.Empty,
                    (x, i) => $"{x} {i} {Environment.NewLine}");
            }
        }

        public IReadOnlyCollection<string> SerialisedRecordErrors()
        {
            return this._etlErrors.Select(i => i.ToString()).ToList();
        }
    }
}