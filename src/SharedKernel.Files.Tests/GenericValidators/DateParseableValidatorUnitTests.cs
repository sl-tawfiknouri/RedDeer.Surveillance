using FluentAssertions;
using FluentValidation;
using NUnit.Framework;
using SharedKernel.Files.ExtendedValidators;
using System.Linq;

namespace SharedKernel.Files.Tests.GenericValidators
{
    public class DateParseableValidatorUnitTests
    {
        private InlineValidator<PropertyValidator> validator;

        [SetUp]
        public void SetUp()
        {
            validator = new InlineValidator<PropertyValidator>();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void DateParseableValidator_WhenDateFieldIsNullOrWhitespace_IsValid(string dateValue)
        {
            validator
                .RuleFor(r => r.PropertyValue)
                .IsParseableDate();

            var propertyValue = new PropertyValidator { PropertyValue = dateValue };

            var result = validator.Validate(propertyValue);

            result.IsValid.Should().BeTrue();
        }

        [TestCase("Random Date Value")]
        public void DateParseableValidator_WhenHasInvalidDateFormat_IsNotValid(string dateValue)
        {
            validator
                .RuleFor(r => r.PropertyValue)
                .IsParseableDate();

            var propertyValue = new PropertyValidator { PropertyValue = dateValue };

            var result = validator.Validate(propertyValue);

            result.IsValid.Should().BeFalse();
            result.Errors
                .Where(w => w.PropertyName == nameof(PropertyValidator.PropertyValue))
                .Where(w => w.ErrorCode == nameof(DateParseableValidator))
                .Should()
                .Contain(w => w.ErrorMessage == $"'{FluentValidation.Internal.Extensions.SplitPascalCase(nameof(PropertyValidator.PropertyValue))}' must have a valid date. '{dateValue}' is not recognised using formats: ['yyyy-MM-ddTHH:mm:ss', 'yyyy-MM-dd'].");
        }

        [TestCase("2009-12-31")]
        [TestCase("2009-12-31T23:59:59")]
        public void DateParseableValidator_WhenHasValidDateFormatButBefore2010_IsNotValid(string dateValue)
        {
            validator
                .RuleFor(r => r.PropertyValue)
                .IsParseableDate();

            var propertyValue = new PropertyValidator { PropertyValue = dateValue };

            var result = validator.Validate(propertyValue);

            result.IsValid.Should().BeFalse();
            result.Errors
                .Where(w => w.PropertyName == nameof(PropertyValidator.PropertyValue))
                .Where(w => w.ErrorCode == nameof(DateParseableValidator))
                .Should()
                .Contain(w => w.ErrorMessage == $"'{FluentValidation.Internal.Extensions.SplitPascalCase(nameof(PropertyValidator.PropertyValue))}' must have a valid date. '{dateValue}' has a year preceding 2010 which is out of range.");
        }

        [TestCase("2010-01-01")]
        [TestCase("2010-01-01T23:59:59")]
        [TestCase("2019-10-31 14:51:57.000")]
        public void DateParseableValidator_WhenHasValidDateFormatAndAfter2010_IsValid(string dateValue)
        {
            validator
                .RuleFor(r => r.PropertyValue)
                .IsParseableDate();

            var propertyValue = new PropertyValidator { PropertyValue = dateValue };

            var result = validator.Validate(propertyValue);

            result.IsValid.Should().BeTrue();
        }

        public class PropertyValidator
        {
            public string PropertyValue { get; set; }
        }
    }
}
