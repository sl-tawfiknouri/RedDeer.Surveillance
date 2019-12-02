using FluentValidation.Validators;
using NUnit.Framework;
using SharedKernel.Files.Orders;
using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;

namespace SharedKernel.Files.Tests.Orders
{
    public partial class BaseOrderFileValidatorUnitTests
    {
        [TestCaseSource(typeof(BaseOrderFileValidatorUnitTests), nameof(LengthValidatorTestCases), new object[] { nameof(OrderFileValidator_LengthValidator) })]
        public void OrderFileValidator_LengthValidator(OrderFileValidatorTestData testData)
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

        public static IEnumerable LengthValidatorTestCases(string testName)
        {
            yield return CreateLengthValidatorValidatorTestCaseData(testName, o => o.InstrumentSedol, 1, 7, new OrderFileContract { InstrumentSedol = CreateStringOfLength(7 + 1) });

            yield return CreateLengthValidatorValidatorTestCaseData(testName, o => o.InstrumentCusip, 6, 9, new OrderFileContract { InstrumentCusip = CreateStringOfLength(6 - 1) });
            yield return CreateLengthValidatorValidatorTestCaseData(testName, o => o.InstrumentCusip, 6, 9, new OrderFileContract { InstrumentCusip = CreateStringOfLength(9 + 1) });

            yield return CreateLengthValidatorValidatorTestCaseData(testName, o => o.InstrumentUnderlyingSedol, 1, 7, new OrderFileContract { InstrumentUnderlyingSedol = CreateStringOfLength(7 + 1) });

            yield return CreateLengthValidatorValidatorTestCaseData(testName, o => o.InstrumentUnderlyingCusip, 6, 9, new OrderFileContract { InstrumentUnderlyingCusip = CreateStringOfLength(6 - 1) });
            yield return CreateLengthValidatorValidatorTestCaseData(testName, o => o.InstrumentUnderlyingCusip, 6, 9, new OrderFileContract { InstrumentUnderlyingCusip = CreateStringOfLength(9 + 1) });

            yield return CreateLengthValidatorValidatorTestCaseData(testName, o => o.InstrumentRic, 1, 30, new OrderFileContract { InstrumentRic = CreateStringOfLength(30 + 1), InstrumentCfi = "D" });
            yield return CreateLengthValidatorValidatorTestCaseData(testName, o => o.InstrumentUnderlyingRic, 1, 30, new OrderFileContract { InstrumentUnderlyingRic = CreateStringOfLength(30 + 1), InstrumentCfi = "D" });
        }

        public static TestCaseData CreateLengthValidatorValidatorTestCaseData(string testName, Expression<Func<OrderFileContract, string>> expression, int min, int max, OrderFileContract orderFileContract)
        {
            var property = GetProperty(expression, orderFileContract);
            var testData = new OrderFileValidatorTestData
            {
                RuleValidator = nameof(LengthValidator),
                ExpectedMessage = $"'{property.PropertyDisplayName}' must be between {min} and {max} characters. You entered {property.PropertyValue.Length} characters.",
                PropertyName = property.PropertyName,
                OrderFileContract = orderFileContract
            };

            return new TestCaseData(testData)
                .SetName($"{testName} when property '{property.PropertyName}' value '{property.PropertyValue ?? "NULL" }' must return '{testData.ExpectedMessage}'");
        }
    }
}