using FluentAssertions;
using FluentValidation.Validators;
using NUnit.Framework;
using SharedKernel.Files.Orders;
using System.Linq;

namespace SharedKernel.Files.Tests.Orders
{
    public partial class BaseOrderFIleValidatorUnitTests
    {
        [TestCase(null, true)]  // Equity
        [TestCase("E", true)]   // Equity
        [TestCase("D", false)]  // Fixed Income
        public void OrderFileValidator_RulesForSufficientInstrumentIdentificationCodes(string instrumentCfi, bool expectedAnyErrors)
        {
            var orderFileContract = new OrderFileContract
            {
                InstrumentIsin = null,
                InstrumentSedol = null,
                InstrumentCusip = null,
                InstrumentBloombergTicker = null,
                InstrumentCfi = instrumentCfi
            };

            var result = validator.Validate(orderFileContract);

            result.IsValid.Should().BeFalse();

            var ExpectedMessage = "At least one of the 'InstrumentIsin; InstrumentSedol; InstrumentCusip; InstrumentBloombergTicker' instrument must be defined.";

            var errors = result.Errors.WherePropertyName("");
            var logErrors = errors.ToList();
            var expectedErrors = errors
                .WhereErrorMessage(ExpectedMessage)
                .WhereRuleValidator(nameof(PredicateValidator));

            var message = FormatMessage(ExpectedMessage, logErrors);
            Assert.AreEqual(expectedAnyErrors, expectedErrors.Any(), message);
        }

        [TestCase(nameof(OrderFileContract.MarketName))]
        [TestCase(nameof(OrderFileContract.MarketIdentifierCode))]
        public void OrderFileValidator_RulesForSufficientInstrumentIdentificationCodess(string propertyName)
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