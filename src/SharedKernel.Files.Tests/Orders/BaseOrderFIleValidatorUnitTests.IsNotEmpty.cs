using Domain.Core.Trading.Orders;
using NUnit.Framework;
using SharedKernel.Files.Orders;
using SharedKernel.Files.ExtendedValidators;
using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;

namespace SharedKernel.Files.Tests.Orders
{
    public partial class BaseOrderFileValidatorUnitTests
    {
        [TestCaseSource(typeof(BaseOrderFileValidatorUnitTests), nameof(PropertyIsNotEmptyTestCases), new object[] { nameof(OrderFileValidator_IsNotEmptyValidator) })]
        public void OrderFileValidator_IsNotEmptyValidator(OrderFileValidatorTestData testData)
        {
            var result = validator.Validate(testData.OrderFileContract);

            var errors = result.Errors.WherePropertyName(testData.PropertyName);
            var logErrors = errors.ToList();
            var expectedErrors = errors
                .WhereErrorMessage(testData.ExpectedMessage)
                .WhereRuleValidator(testData.RuleValidator);

            var message = FormatMessage(testData.ExpectedMessage, logErrors); //
            Assert.IsTrue(expectedErrors.Any(), message);
        }

        public static IEnumerable PropertyIsNotEmptyTestCases(string testName)
        {
            foreach (var value in new string[] { null, "", "  "})
            {
                yield return CreateIsNotEmptyValidatorTestCaseData(testName, o => o.MarketIdentifierCode, new OrderFileContract { MarketIdentifierCode = value });

                yield return CreateIsNotEmptyValidatorTestCaseData(testName, o => o.OrderOrderedVolume, new OrderFileContract { OrderOrderedVolume = value });

                yield return CreateIsNotEmptyValidatorTestCaseData(testName, o => o.OrderId, new OrderFileContract { OrderId = value });
                yield return CreateIsNotEmptyValidatorTestCaseData(testName, o => o.OrderCurrency, new OrderFileContract { OrderCurrency = value });
                yield return CreateIsNotEmptyValidatorTestCaseData(testName, o => o.OrderDirection, new OrderFileContract { OrderDirection = value });

                yield return CreateIsNotEmptyValidatorTestCaseData(testName, o => o.OrderLimitPrice, new OrderFileContract { OrderLimitPrice = value, OrderType = OrderTypes.LIMIT.ToString() }, " When 'OrderType' value is 'LIMIT'"); 

                yield return CreateIsNotEmptyValidatorTestCaseData(testName, o => o.OrderPlacedDate, new OrderFileContract { OrderPlacedDate = value });
                yield return CreateIsNotEmptyValidatorTestCaseData(testName, o => o.InstrumentCfi, new OrderFileContract { InstrumentCfi = value });
            }
        }

        public static TestCaseData CreateIsNotEmptyValidatorTestCaseData(string testName, Expression<Func<OrderFileContract, string>> expression, OrderFileContract orderFileContract, string additionalMessage = null)
        {
            var property = GetProperty(expression, orderFileContract);
            var testData = new OrderFileValidatorTestData
            {
                RuleValidator = nameof(IsNotEmptyValidator),
                ExpectedMessage = $"'{property.PropertyDisplayName}' with value '{property.PropertyValue}' must not be empty.{additionalMessage}",
                PropertyName = property.PropertyName,
                OrderFileContract = orderFileContract
            };

            return new TestCaseData(testData)
                .SetName($"{testName} when property '{property.PropertyName}' value '{property.PropertyValue ?? "NULL" }' must return '{testData.ExpectedMessage}'");
        }
    }
}
