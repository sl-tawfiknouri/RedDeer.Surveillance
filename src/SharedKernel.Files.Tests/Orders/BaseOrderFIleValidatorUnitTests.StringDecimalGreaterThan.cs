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
        [TestCaseSource(typeof(BaseOrderFileValidatorUnitTests), nameof(StringDecimalGreaterThanValidatorTestCases), new object[] { nameof(OrderFileValidator_StringDecimalGreaterThan) })]
        public void OrderFileValidator_StringDecimalGreaterThan(OrderFileValidatorTestData testData)
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

        public static IEnumerable StringDecimalGreaterThanValidatorTestCases(string testName)
        {
            foreach (var value in new string[] { "", " ", "-1.4", "-1", "0", "non-decimal" })
            {
                yield return CreateStringDecimalGreaterThanValidatorTestCaseData(testName, o => o.OrderOrderedVolume, new OrderFileContract { OrderOrderedVolume = value }, null);
                yield return CreateStringDecimalGreaterThanValidatorTestCaseData(testName, o => o.OrderAverageFillPrice, new OrderFileContract { OrderAverageFillPrice = value, OrderFilledDate = "2019-11-26" }, " When 'Order Filled Date' is not null or empty space.");
                yield return CreateStringDecimalGreaterThanValidatorTestCaseData(testName, o => o.OrderFilledVolume, new OrderFileContract { OrderFilledVolume = value, OrderFilledDate = "2019-11-26" }, " When 'Order Filled Date' is not null or empty space.");
            }
        }

        public static TestCaseData CreateStringDecimalGreaterThanValidatorTestCaseData(string testName, Expression<Func<OrderFileContract, string>> expression, OrderFileContract orderFileContract, string additionalMessage)
        {
            var property = GetProperty(expression, orderFileContract);
            var testData = new OrderFileValidatorTestData
            {
                RuleValidator = nameof(StringDecimalGreaterThanValidator),
                ExpectedMessage = $"'{property.PropertyDisplayName}' with value '{property.PropertyValue}' must be greater than '0'.{additionalMessage}",
                PropertyName = property.PropertyName,
                OrderFileContract = orderFileContract
            };

            return new TestCaseData(testData)
                .SetName($"{testName} when property '{property.PropertyName}' value '{property.PropertyValue ?? "NULL" }' must return '{testData.ExpectedMessage}'");
        }
    }
}