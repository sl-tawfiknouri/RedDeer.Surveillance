using System.Collections.Generic;
using DataImport.Disk_IO.EtlFile;
using FluentValidation.Results;
using NUnit.Framework;
using SharedKernel.Files.Orders;

namespace DataImport.Tests.Disk_IO.EtlFile
{
    [TestFixture]
    public class EtlUploadErrorStoreTests
    {
        [Test]
        public void AddNullContract_SerialisedErrors_Empty()
        {
            var store = new EtlUploadErrorStore();
            var validationFailures = new List<ValidationFailure>();

            store.Add(null, validationFailures);
            var errors = store.SerialisedErrors();

            Assert.AreEqual(errors, string.Empty);
        }

        [Test]
        public void AddThenClear_SerialisedErrors_IsEmpty()
        {
            var store = new EtlUploadErrorStore();
            var contract = new OrderFileContract();
            var validationFailures = new List<ValidationFailure>();

            store.Add(contract, validationFailures);
            store.Clear();

            var errors = store.SerialisedErrors();

            Assert.AreEqual(errors, string.Empty);
        }

        [Test]
        public void Add_SerialisedErrors_ExpectedResponse()
        {
            var store = new EtlUploadErrorStore();
            var contract = new OrderFileContract() { OrderId = "12345" };
            var validationFailures = new List<ValidationFailure>();

            store.Add(contract, validationFailures);
            var errors = store.SerialisedErrors();
            var expectedResult =
                " Order id 12345 \r\n  \r\n \r\n";

            Assert.AreEqual(errors, expectedResult);
        }
    }
}
