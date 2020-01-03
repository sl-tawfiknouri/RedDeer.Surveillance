using FluentAssertions;
using FluentValidation.Validators;
using NUnit.Framework;
using SharedKernel.Files.Orders;
using System.Linq;

namespace SharedKernel.Files.Tests.Orders
{
    public partial class BaseOrderFileValidatorUnitTests
    {
        [TestCase(null, null, null, null, null, true, "At least 'InstrumentSedol' or 'InstrumentIsin' with 'MarketIdentifierCode' must be definded for instrument when not fixed income.")]  // Equity
        [TestCase("E", null, null, null, null, true, "At least 'InstrumentSedol' or 'InstrumentIsin' with 'MarketIdentifierCode' must be definded for instrument when not fixed income.")]  // Equity
        [TestCase("E", "MIC", null, null, null, true, "At least 'InstrumentSedol' or 'InstrumentIsin' with 'MarketIdentifierCode' must be definded for instrument when not fixed income.")]  // Equity
        [TestCase("E", null, "ISIN", null, null, true, "At least 'InstrumentSedol' or 'InstrumentIsin' with 'MarketIdentifierCode' must be definded for instrument when not fixed income.")]  // Equity
        [TestCase(null, null, null, "SEDOL", null, false, "")]  // Equity
        [TestCase(null, "MIC", "ISIN", null, null, false, "")]  // Equity
        [TestCase("E", "MIC", "ISIN", null, null, false, "")]  // Equity

        [TestCase("D", "MIC", null, null, null, true, "At least 'InstrumentRic' or 'InstrumentIsin' with 'MarketIdentifierCode' must be definded for instrument when fixed income.")]  // Fixed Income
        [TestCase("D", null, "ISIN", null, null, true, "At least 'InstrumentRic' or 'InstrumentIsin' with 'MarketIdentifierCode' must be definded for instrument when fixed income.")]  // Fixed Income
        [TestCase("D", null, null, null, "RIC", false, "")]  // Fixed Income
        [TestCase("D", "MIC", "ISIN", null, null, false, "")]  // Fixed Income
        public void OrderFileValidator_RulesForSufficientInstrumentIdentificationCodes(string instrumentCfi, string marketIdentifierCode, string instrumentIsin, string instrumentSedol, string instrumentRic, bool expectedAnyErrors, string expectedMessage)
        {
            var orderFileContract = new OrderFileContract
            {
                InstrumentCfi = instrumentCfi,
                MarketIdentifierCode = marketIdentifierCode,
                InstrumentIsin = instrumentIsin,
                InstrumentSedol = instrumentSedol,
                InstrumentRic = instrumentRic
            };

            var result = validator.Validate(orderFileContract);

            result.IsValid.Should().BeFalse();

            var errors = result.Errors.WherePropertyName("");
            var logErrors = errors.ToList();
            var expectedErrors = errors
                .WhereRuleValidator(nameof(PredicateValidator));

            if (expectedAnyErrors)
            {
                expectedErrors = expectedErrors.WhereErrorMessage(expectedMessage);
            }

            var message = FormatMessage(expectedMessage, logErrors);
            Assert.AreEqual(expectedAnyErrors, expectedErrors.Any(), message);
        }

        [TestCase(nameof(OrderFileContract.MarketName))]
        [TestCase(nameof(OrderFileContract.MarketIdentifierCode))]
        public void OrderFileValidator_RulesForSufficientInstrumentIdentificationCodes(string propertyName)
        {
            var expected = "RDFI";
            var orderFileContract = new OrderFileContract
            {
                InstrumentCfi = "D",
                MarketIdentifierCode = null,
                MarketName = null
            };

            var result = validator.Validate(orderFileContract);

            result.IsValid.Should().BeFalse();

            var expectedMessage = $"'{FluentValidation.Internal.Extensions.SplitPascalCase(propertyName)}' must be equal to '{expected}'. When fixed income.";

            var errors = result.Errors.WherePropertyName(propertyName);
            var logErrors = errors.ToList();
            var expectedErrors = errors
                .WhereErrorMessage(expectedMessage)
                .WhereRuleValidator(nameof(EqualValidator));

            var message = FormatMessage(expectedMessage, logErrors);
            Assert.IsTrue(expectedErrors.Any(), message);
        }
    }
}