using FluentValidation.Validators;
using NUnit.Framework;
using SharedKernel.Files.Orders;
using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;

namespace SharedKernel.Files.Tests.Orders
{
    public partial class BaseOrderFIleValidatorUnitTests
    {
        [TestCaseSource(typeof(BaseOrderFIleValidatorUnitTests), nameof(PropertyMaximumLengthTestCases), new object[] { nameof(OrderFileValidator_MaximumLengthValidator) })]
        public void OrderFileValidator_MaximumLengthValidator(OrderFileValidatorTestData testData)
        {
            var result = validator.Validate(testData.OrderFileContract);

            var errors = result.Errors.WherePropertyName(testData.PropertyName);
            var logErrors = errors.ToList();
            var expectedErrors = errors
                .WhereErrorMessage(testData.ExpectedMessage)
                .WhereRuleValidator(testData.RuleValidator);

            var message = FormatMessage(testData.ExpectedMessage, logErrors);
            Assert.IsTrue(expectedErrors.Any(), message);
        }

        public static IEnumerable PropertyMaximumLengthTestCases(string testName)
        {
            yield return CreateMaximumLengthValidatorTestCaseData(testName, o => o.OrderId, new OrderFileContract { OrderId = CreateStringOfLength(255 + 1) });
            yield return CreateMaximumLengthValidatorTestCaseData(testName, o => o.OrderTraderId, new OrderFileContract { OrderTraderId = CreateStringOfLength(255 + 1) });
            yield return CreateMaximumLengthValidatorTestCaseData(testName, o => o.OrderVersion, new OrderFileContract { OrderVersion = CreateStringOfLength(255 + 1) });
            yield return CreateMaximumLengthValidatorTestCaseData(testName, o => o.OrderVersionLinkId, new OrderFileContract { OrderVersionLinkId = CreateStringOfLength(255 + 1) });
            yield return CreateMaximumLengthValidatorTestCaseData(testName, o => o.OrderGroupId, new OrderFileContract { OrderGroupId = CreateStringOfLength(255 + 1) });
            yield return CreateMaximumLengthValidatorTestCaseData(testName, o => o.OrderBroker, new OrderFileContract { OrderBroker = CreateStringOfLength(1023 + 1) });

            yield return CreateMaximumLengthValidatorTestCaseData(testName, o => o.OrderTraderName, new OrderFileContract { OrderTraderName = CreateStringOfLength(255 + 1) });
            yield return CreateMaximumLengthValidatorTestCaseData(testName, o => o.OrderClearingAgent, new OrderFileContract { OrderClearingAgent = CreateStringOfLength(255 + 1) });
            yield return CreateMaximumLengthValidatorTestCaseData(testName, o => o.OrderDealingInstructions, new OrderFileContract { OrderDealingInstructions = CreateStringOfLength(4095 + 1) });

            yield return CreateMaximumLengthValidatorTestCaseData(testName, o => o.InstrumentCfi, new OrderFileContract { InstrumentCfi = CreateStringOfLength(6 + 1) });

            yield return CreateMaximumLengthValidatorTestCaseData(testName, o => o.InstrumentExchangeSymbol, new OrderFileContract { InstrumentExchangeSymbol = CreateStringOfLength(100 + 1) });
            yield return CreateMaximumLengthValidatorTestCaseData(testName, o => o.InstrumentUnderlyingExchangeSymbol, new OrderFileContract { InstrumentUnderlyingExchangeSymbol = CreateStringOfLength(100 + 1) });

        }

        public static TestCaseData CreateMaximumLengthValidatorTestCaseData(string testName, Expression<Func<OrderFileContract, string>> expression, OrderFileContract orderFileContract)
        {
            var property = GetProperty(expression, orderFileContract);
            var testData = new OrderFileValidatorTestData
            {
                RuleValidator = nameof(MaximumLengthValidator),
                ExpectedMessage = $"The length of '{property.PropertyDisplayName}' must be {property.PropertyValue.Length - 1} characters or fewer. You entered {property.PropertyValue.Length} characters.",
                PropertyName = property.PropertyName,
                OrderFileContract = orderFileContract
            };

            return new TestCaseData(testData)
                .SetName($"{testName} when property '{property.PropertyName}' value '{property.PropertyValue ?? "NULL" }' must return '{testData.ExpectedMessage}'");
        }
    }
}