namespace DataImport.Tests.Disk_IO.EtlFile
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using DataImport.Disk_IO.EtlFile;

    using FluentValidation.Results;

    using NUnit.Framework;

    using SharedKernel.Files.Orders;

    [TestFixture]
    public class EtlUploadErrorStoreTests
    {
        [Test]
        public void Add_ManySerialisedErrors_ExpectedResponse()
        {
            var store = new EtlUploadErrorStore();
            var contract = new OrderFileContract { OrderId = "12345" };
            var validationFailures = new List<ValidationFailure>();
            store.Add(contract, validationFailures);

            var contract1 = new OrderFileContract { OrderId = "123456" };
            var validationFailures1 = new List<ValidationFailure>();
            store.Add(contract1, validationFailures1);

            var contract2 = new OrderFileContract { OrderId = "1234567" };
            var validationFailures2 = new List<ValidationFailure>
                                          {
                                              new ValidationFailure("prop-1", "error-1"),
                                              new ValidationFailure("prop-2", "error-2")
                                          };

            store.Add(contract2, validationFailures2);

            var errors = store.SerialisedErrors(2).ToList();

            var expectedResult1 =
                $" Order id 12345 {Environment.NewLine}  {Environment.NewLine} {Environment.NewLine} Order id 123456 {Environment.NewLine}  {Environment.NewLine} {Environment.NewLine}";

            var expectedResult2 =
                $" Order id 1234567 {Environment.NewLine}  prop-1. error-1 {Environment.NewLine} prop-2. error-2 {Environment.NewLine} {Environment.NewLine} {Environment.NewLine}";

            Assert.AreEqual(errors.FirstOrDefault(), expectedResult1);
            Assert.AreEqual(errors.Skip(1).FirstOrDefault(), expectedResult2);
        }

        [Test]
        public void Add_SerialisedErrors_ExpectedResponse()
        {
            var store = new EtlUploadErrorStore();
            var contract = new OrderFileContract { OrderId = "12345" };
            var validationFailures = new List<ValidationFailure>();

            store.Add(contract, validationFailures);
            var errors = store.SerialisedErrors();
            var expectedResult = $" Order id 12345 {Environment.NewLine}  {Environment.NewLine} {Environment.NewLine}";

            Assert.AreEqual(errors, expectedResult);
        }

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
    }
}