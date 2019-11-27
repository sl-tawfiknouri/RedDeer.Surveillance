using FluentAssertions;
using NUnit.Framework;
using SharedKernel.Files.Orders;
using System.Linq;

namespace SharedKernel.Files.Tests
{
    public partial class BaseOrderFIleValidatorUnitTests
    {
        [Test]
        public void OrderFileValidator_RulesForSufficientInstrumentIdentificationCodes()
        {
            var orderFileContract = new OrderFileContract
            {
                InstrumentIsin = null,
                InstrumentSedol = null,
                InstrumentCusip = null,
                InstrumentBloombergTicker = null
            };

            var result = validator.Validate(orderFileContract);

            result.IsValid.Should().BeFalse();

            var ExpectedMessage = "At least one of the 'InstrumentIsin; InstrumentSedol; InstrumentCusip; InstrumentBloombergTicker' instrument must be defined.";

            var errors = result.Errors.WherePropertyName("");
            var logErrors = errors.ToList();
            var expectedErrors = errors
                .WhereErrorMessage(ExpectedMessage)
                .WhereRuleValidator("PredicateValidator");

            var message = FormatMessage(ExpectedMessage, logErrors);
            Assert.IsTrue(expectedErrors.Any(), message);
        }
    }
}