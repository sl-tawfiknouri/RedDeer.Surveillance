using System.Collections.Generic;
using FluentValidation.Results;
using SharedKernel.Files.Orders;

namespace DataImport.Disk_IO.EtlFile.Interfaces
{
    public interface IEtlUploadErrorStore
    {
        void Add(OrderFileContract record, IList<ValidationFailure> validationFailures);
        void Clear();
        string SerialisedErrors();
        IReadOnlyCollection<string> SerialisedRecordErrors();
        IEnumerable<string> SerialisedErrors(int segmentLimit);
    }
}