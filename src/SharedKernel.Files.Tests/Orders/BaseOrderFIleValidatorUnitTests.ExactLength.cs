using FluentValidation.Validators;
using NUnit.Framework;
using SharedKernel.Files.Orders;
using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;

namespace SharedKernel.Files.Tests
{
    public partial class BaseOrderFIleValidatorUnitTests
    {
        [TestCaseSource(typeof(BaseOrderFIleValidatorUnitTests), nameof(ExactLengthValidatorTestCases), new object[] { nameof(OrderFileValidator_ExactLengthValidator) })]
        public void OrderFileValidator_ExactLengthValidator(OrderFileValidatorTestData testData)
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

        public static IEnumerable ExactLengthValidatorTestCases(string testName)
        {
            foreach (var item in new int[] { -1, 1 })
            {
                yield return CreateExactLengthValidatorValidatorTestCaseData(testName, o => o.OrderCurrency, 3, new OrderFileContract { OrderCurrency = CreateStringOfLength(3 - item) });
                yield return CreateExactLengthValidatorValidatorTestCaseData(testName, o => o.OrderSettlementCurrency, 3, new OrderFileContract { OrderSettlementCurrency = CreateStringOfLength(3 - item) });

                yield return CreateExactLengthValidatorValidatorTestCaseData(testName, o => o.InstrumentIsin, 12, new OrderFileContract { InstrumentIsin = CreateStringOfLength(12 + item) });
                yield return CreateExactLengthValidatorValidatorTestCaseData(testName, o => o.InstrumentLei, 20, new OrderFileContract { InstrumentLei = CreateStringOfLength(20 + item) });
                yield return CreateExactLengthValidatorValidatorTestCaseData(testName, o => o.InstrumentFigi, 12, new OrderFileContract { InstrumentFigi = CreateStringOfLength(12 + item) });
                yield return CreateExactLengthValidatorValidatorTestCaseData(testName, o => o.InstrumentUnderlyingIsin, 12, new OrderFileContract { InstrumentUnderlyingIsin = CreateStringOfLength(12 + item) });
                yield return CreateExactLengthValidatorValidatorTestCaseData(testName, o => o.InstrumentUnderlyingLei, 20, new OrderFileContract { InstrumentUnderlyingLei = CreateStringOfLength(20 + item) });
                yield return CreateExactLengthValidatorValidatorTestCaseData(testName, o => o.InstrumentUnderlyingFigi, 12, new OrderFileContract { InstrumentUnderlyingFigi = CreateStringOfLength(12 + item) });
            }
        }

        public static TestCaseData CreateExactLengthValidatorValidatorTestCaseData(string testName, Expression<Func<OrderFileContract, string>> expression, int expectedLength, OrderFileContract orderFileContract)
        {
            var property = GetProperty(expression, orderFileContract);
            var testData = new OrderFileValidatorTestData
            {
                RuleValidator = nameof(ExactLengthValidator),
                ExpectedMessage = $"'{property.PropertyDisplayName}' must be {expectedLength} characters in length. You entered {property.PropertyValue.Length} characters.",
                PropertyName = property.PropertyName,
                OrderFileContract = orderFileContract
            };

            return new TestCaseData(testData)
                .SetName($"{testName} when property '{property.PropertyName}' value '{property.PropertyValue ?? "NULL" }' must return '{testData.ExpectedMessage}'");
        }
    }
}