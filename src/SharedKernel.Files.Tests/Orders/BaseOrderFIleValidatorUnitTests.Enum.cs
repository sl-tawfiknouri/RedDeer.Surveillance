using Domain.Core.Markets;
using Domain.Core.Trading.Orders;
using FluentValidation.Validators;
using NUnit.Framework;
using SharedKernel.Files.Orders;
using SharedKernel.Files.PropertyValidators;
using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;

namespace SharedKernel.Files.Tests
{
    public partial class BaseOrderFIleValidatorUnitTests
    {
        [TestCaseSource(typeof(BaseOrderFIleValidatorUnitTests), nameof(EnumParseableValidatorTestCases), new object[] { nameof(OrderFileValidator_EnumParseableValidator) })]
        public void OrderFileValidator_EnumParseableValidator(OrderFileValidatorTestData testData)
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

        public static IEnumerable EnumParseableValidatorTestCases(string testName)
        {
            yield return CreateEnumParseableValidatorTestCaseData<MarketTypes>(testName, o => o.MarketType, new OrderFileContract { MarketType = "NON-EXISTING-ENUM" });
            yield return CreateEnumParseableValidatorTestCaseData<OrderTypes>(testName, o => o.OrderType, new OrderFileContract { OrderType = "NON-EXISTING-ENUM" });
            yield return CreateEnumParseableValidatorTestCaseData<OrderDirections>(testName, o => o.OrderDirection, new OrderFileContract { OrderDirection = "NON-EXISTING-ENUM" });
            yield return CreateEnumParseableValidatorTestCaseData<OrderCleanDirty>(testName, o => o.OrderCleanDirty, new OrderFileContract { OrderCleanDirty = "NON-EXISTING-ENUM" });
        }

        public static TestCaseData CreateEnumParseableValidatorTestCaseData<TEnum>(string testName, Expression<Func<OrderFileContract, string>> expression, OrderFileContract orderFileContract)
            where TEnum : struct, IConvertible
        {
            var property = GetProperty(expression, orderFileContract);
            var testData = new OrderFileValidatorTestData
            {
                RuleValidator = typeof(EnumParseableValidator<TEnum>).Name,
                ExpectedMessage = $"'{property.PropertyDisplayName}' value '{property.PropertyValue}' must be assignable to typeof<{typeof(TEnum)}> enum.",
                PropertyName = property.PropertyName,
                OrderFileContract = orderFileContract
            };

            return new TestCaseData(testData)
                .SetName($"{testName} when property '{property.PropertyName}' value '{property.PropertyValue ?? "NULL" }' must return '{testData.ExpectedMessage}'");
        }

        [TestCaseSource(typeof(BaseOrderFIleValidatorUnitTests), nameof(ValidEnumParseableValidatorTestCases), new object[] { nameof(OrderFileValidator_EnumParseableValidator_WhenValid) })]
        public void OrderFileValidator_EnumParseableValidator_WhenValid(OrderFileValidatorTestData testData)
        {
            var result = validator.Validate(testData.OrderFileContract);

            var errors = result.Errors.WherePropertyName(testData.PropertyName);
            var logErrors = errors.ToList();
            
            var message = FormatMessage(testData.ExpectedMessage, logErrors);
            Assert.IsFalse(errors.Any(), message);
        }

        public static IEnumerable ValidEnumParseableValidatorTestCases(string testName)
        {
            foreach (var enumValue in GetEnumValidValues<MarketTypes>())
                yield return CreateEnumParseableValidatorTestCaseData<MarketTypes>(testName, o => o.MarketType, new OrderFileContract { MarketType = enumValue });

            foreach (var enumValue in GetEnumValidValues<OrderTypes>())
                yield return CreateEnumParseableValidatorTestCaseData<OrderTypes>(testName, o => o.OrderType, new OrderFileContract { OrderType = enumValue });
            
            foreach (var enumValue in GetEnumValidValues<OrderDirections>().Where(w =>!new string[] { ((int)OrderDirections.NONE).ToString(), OrderDirections.NONE.ToString() }.Contains(w)))
                yield return CreateEnumParseableValidatorTestCaseData<OrderDirections>(testName, o => o.OrderDirection, new OrderFileContract { OrderDirection = enumValue });

            foreach (var enumValue in GetEnumValidValues<OrderCleanDirty>())
                yield return CreateEnumParseableValidatorTestCaseData<OrderCleanDirty>(testName, o => o.OrderCleanDirty, new OrderFileContract { OrderCleanDirty = enumValue });
        }

        public static TestCaseData CreateValidEnumParseableValidatorTestCaseData<TEnum>(string testName, Expression<Func<OrderFileContract, string>> expression, OrderFileContract orderFileContract)
            where TEnum : struct, IConvertible
        {
            var property = GetProperty(expression, orderFileContract);
            var testData = new OrderFileValidatorTestData
            {
                RuleValidator = typeof(EnumParseableValidator<TEnum>).Name,
                PropertyName = property.PropertyName,
                OrderFileContract = orderFileContract
            };

            return new TestCaseData(testData)
                .SetName($"{testName} when property '{property.PropertyName}' value '{property.PropertyValue ?? "NULL" }' must be valid'");
        }

        [TestCaseSource(typeof(BaseOrderFIleValidatorUnitTests), nameof(NonValidEnumParseableValidatorTestCases), new object[] { nameof(OrderFileValidator_EnumParseableValidator_WhenNonValid) })]
        public void OrderFileValidator_EnumParseableValidator_WhenNonValid(OrderFileValidatorTestData testData)
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

        public static IEnumerable NonValidEnumParseableValidatorTestCases(string testName)
        {
            foreach (var enumValue in GetEnumValidValues<OrderDirections>().Where(w => new string[] { ((int)OrderDirections.NONE).ToString(), OrderDirections.NONE.ToString() }.Contains(w)))
                yield return CreateNonValidEnumParseableValidatorTestCaseData<OrderDirections>(testName, o => o.OrderDirection, new OrderFileContract { OrderDirection = enumValue });
        }

        public static TestCaseData CreateNonValidEnumParseableValidatorTestCaseData<TEnum>(string testName, Expression<Func<OrderFileContract, string>> expression, OrderFileContract orderFileContract)
            where TEnum : struct, IConvertible
        {
            var property = GetProperty(expression, orderFileContract);
            var testData = new OrderFileValidatorTestData
            {
                RuleValidator = nameof(NotEqualValidator),
                PropertyName = property.PropertyName,
                OrderFileContract = orderFileContract,
                ExpectedMessage = $"'{property.PropertyDisplayName}' must not be equal to '{property.PropertyValue}'."
            };

            return new TestCaseData(testData)
                .SetName($"{testName} when property '{property.PropertyName}' value '{property.PropertyValue ?? "NULL" }' must not be equal to '{property.PropertyValue}'");
        }

    }
}