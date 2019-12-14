using FluentAssertions;
using FluentValidation;
using NUnit.Framework;
using SharedKernel.Files.ExtendedValidators;
using System.Linq;

namespace SharedKernel.Files.Tests.GenericValidators
{
    public class StringDecimalGreaterThanValidatorUnitTests
    {
        private InlineValidator<PropertyValidator> validator;

        [SetUp]
        public void SetUp()
        {
            validator = new InlineValidator<PropertyValidator>();
        }


        [TestCase("1", 0)]
        public void StringDecimalgGreaterThanValidator_WhenValidDecimalStringAndGreaterThan(string value, long greaterThan)
        {
            validator
                .RuleFor(r => r.PropertyValue)
                .StringDecimalGreaterThan(greaterThan);

            var propertyValue = new PropertyValidator { PropertyValue = value };

            var result = validator.Validate(propertyValue);

            result.IsValid.Should().BeTrue();
            
        }

        [TestCase("0", 1)]
        [TestCase("1", 1)]
        public void StringDecimalGreaterThanValidator_WhenDecimalStringOrNotGreaterThan(string value, long greaterThan)
        {
            validator
                .RuleFor(r => r.PropertyValue)
                .StringDecimalGreaterThan(greaterThan);

            var propertyValue = new PropertyValidator { PropertyValue = value };

            var result = validator.Validate(propertyValue);

            result.IsValid.Should().BeFalse();
            result.Errors
                .Where(w => w.PropertyName == nameof(PropertyValidator.PropertyValue))
                .Where(w => w.ErrorCode == nameof(StringDecimalGreaterThanValidator))
                .Should()
                .Contain(w => w.ErrorMessage == $"'{FluentValidation.Internal.Extensions.SplitPascalCase(nameof(PropertyValidator.PropertyValue))}' with value '{value}' must be greater than '{greaterThan}'.");
        }

        public class PropertyValidator
        {
            public string PropertyValue { get; set; }
        }
    }
}
