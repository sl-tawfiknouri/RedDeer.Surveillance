using NUnit.Framework;
using SharedKernel.Files.Orders;
using SharedKernel.Files.ExtendedValidators;
using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;

namespace SharedKernel.Files.Tests.Orders
{
    public partial class BaseOrderFIleValidatorUnitTests
    {
        [TestCaseSource(typeof(BaseOrderFIleValidatorUnitTests), nameof(IsDecimalStringValidatorTestCases), new object[] { nameof(OrderFileValidator_IsDecimalStringValidator) })]
        public void OrderFileValidator_IsDecimalStringValidator(OrderFileValidatorTestData testData)
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

        public static IEnumerable IsDecimalStringValidatorTestCases(string testName)
        {
            foreach (var value in new string[] { "non-decimal" })
            {
                yield return CreateIsDecimalStringValidatorTestCaseData(testName, o => o.OrderLimitPrice, new OrderFileContract { OrderLimitPrice = value }, null);
                yield return CreateIsDecimalStringValidatorTestCaseData(testName, o => o.OrderAccumulatedInterest, new OrderFileContract { OrderAccumulatedInterest = value, OrderFilledDate = "2019-11-26" }, null);
                yield return CreateIsDecimalStringValidatorTestCaseData(testName, o => o.OrderAverageFillPrice, new OrderFileContract { OrderAverageFillPrice = value, OrderFilledDate = "2019-11-26" }, " When 'Order Filled Date' is not null or empty space.");
            }

            foreach (var value in new string[] { null, "", " ", "non-decimal" })
            {
                yield return CreateIsDecimalStringValidatorTestCaseData(testName, o => o.OrderFilledVolume, new OrderFileContract { OrderFilledVolume = value, OrderFilledDate = "2019-11-26" }, " When 'Order Filled Date' is not null or empty space.");
            }
        }

        public static TestCaseData CreateIsDecimalStringValidatorTestCaseData(string testName, Expression<Func<OrderFileContract, string>> expression, OrderFileContract orderFileContract, string additionalMessage)
        {
            var property = GetProperty(expression, orderFileContract);
            var testData = new OrderFileValidatorTestData
            {
                RuleValidator = nameof(IsDecimalStringValidator),
                
                ExpectedMessage = $"'{property.PropertyDisplayName}' value '{property.PropertyValue}' must be a valid decimal type.{additionalMessage}",
                PropertyName = property.PropertyName,
                OrderFileContract = orderFileContract
            };

            return new TestCaseData(testData)
                .SetName($"{testName} when property '{property.PropertyName}' value '{property.PropertyValue ?? "NULL" }' must return '{testData.ExpectedMessage}'. {additionalMessage ?? "AdditionalMessage is NULL"}");
        }
    }
}