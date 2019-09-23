namespace DataImport.Tests.Disk_IO.EtlFile
{
    using System;
    using System.Collections.Generic;

    using DataImport.Disk_IO.EtlFile;

    using FluentValidation.Results;

    using NUnit.Framework;

    using SharedKernel.Files.Orders;

    [TestFixture]
    public class EtlErrorRecordTests
    {
        [Test]
        public void RecordWithFailure_ToString_YieldsStringOutput()
        {
            var validationFailures = new List<ValidationFailure>
                                         {
                                             new ValidationFailure("Apple", "Expected type fruit, not company"),
                                             new ValidationFailure(
                                                 "Java",
                                                 "Expected type location, not programming language")
                                         };

            var record = new EtlErrorRecord(new OrderFileContract { OrderId = "12345" }, validationFailures);

            var recordStr = record.ToString();
            var expectedResponse =
                $"Order id 12345 {Environment.NewLine}  Apple. Expected type fruit, not company {Environment.NewLine} Java. Expected type location, not programming language {Environment.NewLine} {Environment.NewLine}";

            Assert.AreEqual(recordStr, expectedResponse);
        }

        [Test]
        public void RecordWithNoFailures_ToString_YieldsStringOutput()
        {
            var record = new EtlErrorRecord(new OrderFileContract { OrderId = "23456" }, new List<ValidationFailure>());

            var recordStr = record.ToString();
            var expectedResponse = $"Order id 23456 {Environment.NewLine}  {Environment.NewLine}";

            Assert.AreEqual(recordStr, expectedResponse);
        }

        [Test]
        public void RecordWithSingleFailure_ToString_YieldsStringOutput()
        {
            var validationFailures = new List<ValidationFailure>
                                         {
                                             new ValidationFailure("Apple", "Expected type fruit, not company")
                                         };

            var record = new EtlErrorRecord(new OrderFileContract { OrderId = "54321" }, validationFailures);

            var recordStr = record.ToString();
            var expectedResponse =
                $"Order id 54321 {Environment.NewLine}  Apple. Expected type fruit, not company {Environment.NewLine} {Environment.NewLine}";

            Assert.AreEqual(recordStr, expectedResponse);
        }
    }
}