﻿using System;
using System.Collections.Generic;
using System.Linq;
using DataImport.Disk_IO.EtlFile.Interfaces;
using FluentValidation.Results;
using SharedKernel.Files.Orders;

namespace DataImport.Disk_IO.EtlFile
{
    public class EtlUploadErrorStore : IEtlUploadErrorStore
    {
        private IList<IEtlErrorRecord> _etlErrors = new List<IEtlErrorRecord>();

        public void Add(
            OrderFileContract record,
            IList<ValidationFailure> validationFailures)
        {
            if (record == null)
            {
                return;
            }

            var etlRecord = new EtlErrorRecord(record, validationFailures);
            _etlErrors.Add(etlRecord);
        }

        public void Clear()
        {
            _etlErrors = new List<IEtlErrorRecord>();
        }

        public IReadOnlyCollection<string> SerialisedRecordErrors()
        {
            return _etlErrors.Select(i => i.ToString()).ToList();
        }

        public string SerialisedErrors()
        {
            return _etlErrors.Select(i => i.ToString()).Aggregate(string.Empty, (x, i) => $"{x} {i} {Environment.NewLine}");
        }

        public IEnumerable<string> SerialisedErrors(int segmentLimit)
        {
            var listCnt = _etlErrors.Count;
            var iterations = 0;

            while (listCnt > 0)
            {
                listCnt -= segmentLimit;
                var errors = _etlErrors.Skip(iterations * segmentLimit).Take(segmentLimit);
                iterations++;

                yield return errors.Select(i => i.ToString()).Aggregate(string.Empty, (x, i) => $"{x} {i} {Environment.NewLine}");
            }
        }
    }
}