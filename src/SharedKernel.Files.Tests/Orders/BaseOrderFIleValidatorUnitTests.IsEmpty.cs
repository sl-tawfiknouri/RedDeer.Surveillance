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
        [TestCaseSource(typeof(BaseOrderFileValidatorUnitTests), nameof(PropertyIsEmptyTestCases), new object[] { nameof(OrderFileValidator_IsEmptyValidator) })]
        public void OrderFileValidator_IsEmptyValidator(OrderFileValidatorTestData testData)
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

        public static IEnumerable PropertyIsEmptyTestCases(string testName)
        {
            yield return CreateIsEmptyValidatorTestCaseData(testName, o => o.DealerOrderId, new OrderFileContract { DealerOrderId = " " });
            yield return CreateIsEmptyValidatorTestCaseData(testName, o => o.DealerOrderCurrency, new OrderFileContract { DealerOrderCurrency = " " });
            yield return CreateIsEmptyValidatorTestCaseData(testName, o => o.DealerOrderPlacedDate, new OrderFileContract { DealerOrderPlacedDate = " " });
            yield return CreateIsEmptyValidatorTestCaseData(testName, o => o.DealerOrderBookedDate, new OrderFileContract { DealerOrderBookedDate = " " });
            yield return CreateIsEmptyValidatorTestCaseData(testName, o => o.DealerOrderVersion, new OrderFileContract { DealerOrderVersion = " " });
            yield return CreateIsEmptyValidatorTestCaseData(testName, o => o.DealerOrderVersionLinkId, new OrderFileContract { DealerOrderVersionLinkId = " " }); 
            yield return CreateIsEmptyValidatorTestCaseData(testName, o => o.DealerOrderGroupId, new OrderFileContract { DealerOrderGroupId = " " });
            yield return CreateIsEmptyValidatorTestCaseData(testName, o => o.DealerOrderDealerId, new OrderFileContract { DealerOrderDealerId = " " });
            yield return CreateIsEmptyValidatorTestCaseData(testName, o => o.DealerOrderNotes, new OrderFileContract { DealerOrderNotes = " " });
            yield return CreateIsEmptyValidatorTestCaseData(testName, o => o.DealerOrderCounterParty, new OrderFileContract { DealerOrderCounterParty = " " });
            yield return CreateIsEmptyValidatorTestCaseData(testName, o => o.DealerOrderSettlementCurrency, new OrderFileContract { DealerOrderSettlementCurrency = " " });
            yield return CreateIsEmptyValidatorTestCaseData(testName, o => o.DealerOrderDealerName, new OrderFileContract { DealerOrderDealerName = " " });
            yield return CreateIsEmptyValidatorTestCaseData(testName, o => o.DealerOrderCleanDirty, new OrderFileContract { DealerOrderCleanDirty = " " });
            yield return CreateIsEmptyValidatorTestCaseData(testName, o => o.DealerOrderType, new OrderFileContract { DealerOrderType = " " });
            yield return CreateIsEmptyValidatorTestCaseData(testName, o => o.DealerOrderDirection, new OrderFileContract { DealerOrderDirection = " " });
            yield return CreateIsEmptyValidatorTestCaseData(testName, o => o.DealerOrderLimitPrice, new OrderFileContract { DealerOrderLimitPrice = " " });
            yield return CreateIsEmptyValidatorTestCaseData(testName, o => o.DealerOrderAverageFillPrice, new OrderFileContract { DealerOrderAverageFillPrice = " " });
            yield return CreateIsEmptyValidatorTestCaseData(testName, o => o.DealerOrderOrderedVolume, new OrderFileContract { DealerOrderOrderedVolume = " " });
            yield return CreateIsEmptyValidatorTestCaseData(testName, o => o.DealerOrderFilledVolume, new OrderFileContract { DealerOrderFilledVolume = " " });
            yield return CreateIsEmptyValidatorTestCaseData(testName, o => o.DealerOrderAccumulatedInterest, new OrderFileContract { DealerOrderAccumulatedInterest = " " });
            yield return CreateIsEmptyValidatorTestCaseData(testName, o => o.DealerOrderAmendedDate, new OrderFileContract { DealerOrderAmendedDate = " " });
            yield return CreateIsEmptyValidatorTestCaseData(testName, o => o.DealerOrderCancelledDate, new OrderFileContract { DealerOrderCancelledDate = " " });
            yield return CreateIsEmptyValidatorTestCaseData(testName, o => o.DealerOrderRejectedDate, new OrderFileContract { DealerOrderRejectedDate = " " });
            yield return CreateIsEmptyValidatorTestCaseData(testName, o => o.DealerOrderFilledDate, new OrderFileContract { DealerOrderFilledDate = " " });
            yield return CreateIsEmptyValidatorTestCaseData(testName, o => o.DealerOrderOptionExpirationDate, new OrderFileContract { DealerOrderOptionExpirationDate = " " });
            yield return CreateIsEmptyValidatorTestCaseData(testName, o => o.DealerOrderOptionEuropeanAmerican, new OrderFileContract { DealerOrderOptionEuropeanAmerican = " " });
            yield return CreateIsEmptyValidatorTestCaseData(testName, o => o.DealerOrderOptionStrikePrice, new OrderFileContract { DealerOrderOptionStrikePrice = " " });
        }

        public static TestCaseData CreateIsEmptyValidatorTestCaseData(string testName, Expression<Func<OrderFileContract, string>> expression, OrderFileContract orderFileContract)
        {
            var property = GetProperty(expression, orderFileContract);
            var testData = new OrderFileValidatorTestData
            {
                RuleValidator = nameof(IsEmptyValidator),
                ExpectedMessage = $"'{property.PropertyDisplayName}' with value '{property.PropertyValue}' must be empty.",
                PropertyName = property.PropertyName,
                OrderFileContract = orderFileContract
            };

            return new TestCaseData(testData)
                .SetName($"{testName} when property '{property.PropertyName}' value '{property.PropertyValue ?? "NULL" }' must return '{testData.ExpectedMessage}'");
        }
    }
}
