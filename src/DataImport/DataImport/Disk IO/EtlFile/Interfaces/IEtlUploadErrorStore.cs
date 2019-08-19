namespace DataImport.Disk_IO.EtlFile.Interfaces
{
    using System.Collections.Generic;

    using FluentValidation.Results;

    using SharedKernel.Files.Orders;

    public interface IEtlUploadErrorStore
    {
        void Add(OrderFileContract record, IList<ValidationFailure> validationFailures);

        void Clear();

        string SerialisedErrors();

        IEnumerable<string> SerialisedErrors(int segmentLimit);

        IReadOnlyCollection<string> SerialisedRecordErrors();
    }
}