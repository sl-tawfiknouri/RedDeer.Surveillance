using FluentAssertions;
using FluentValidation;
using NUnit.Framework;
using SharedKernel.Files.ExtendedValidators;
using System.Linq;

namespace SharedKernel.Files.Tests.GenericValidators
{
    public class IsDecimalValidatorUnitTests
    {
        private InlineValidator<PropertyValidator> validator;

        [SetUp]
        public void SetUp()
        {
            validator = new InlineValidator<PropertyValidator>();
        }

        [Test]
        public void IsDecimalValidator_WhenPropertyNull_IsNotValid()
        {
            validator
                .RuleFor(x => x.PropertyValue)
                .IsDecimal();

            var validationResult = validator.Validate(new PropertyValidator { PropertyValue = null });
            validationResult.IsValid.Should().BeFalse();

            var error = validationResult.Errors.Single();

            error.PropertyName.Should().Be(nameof(PropertyValidator.PropertyValue));
            error.ErrorMessage.Should().Be("'Property Value' value '' must be a valid decimal type.");
        }

        [Test]
        public void IsDecimalValidator_WhenPropertyNonDecimal_IsNotValid()
        {
            validator
                .RuleFor(x => x.PropertyValue)
                .IsDecimal();

            var validationResult = validator.Validate(new PropertyValidator { PropertyValue = "non decimal value" });
            validationResult.IsValid.Should().BeFalse();

            var error = validationResult.Errors.Single();

            error.PropertyName.Should().Be(nameof(PropertyValidator.PropertyValue));
            error.ErrorMessage.Should().Be("'Property Value' value 'non decimal value' must be a valid decimal type.");
        }

        [Test]
        public void IsDecimalValidator_WhenPropertyHasDecimalValue_IsValid()
        {
            validator
                .RuleFor(x => x.PropertyValue)
                .IsDecimal();

            foreach (var item in new [] { decimal.MinValue.ToString(), decimal.MaxValue.ToString(), "0", "0.0" })
            {
                var validationResult = validator.Validate(new PropertyValidator { PropertyValue = item });

                validationResult.IsValid.Should().BeTrue($"Actual value: '{item}'");
                validationResult.Errors.Should().BeEmpty();
            }
        }
        
        [TestCase(null)]
        [TestCase("  ")]
        [TestCase("    ")]
        public void IsDecimalValidator_WhenPropertyIsEmptyOrWhiteSpace_IsValid(string propertyValue)
        {
            validator
                .RuleFor(x => x.PropertyValue)
                .IsDecimal()
                .WhenIsNotNullOrWhitespace();

            var validationResult = validator.Validate(new PropertyValidator { PropertyValue = propertyValue });

            validationResult.IsValid.Should().BeTrue($"Actual value: '{propertyValue}'");
            validationResult.Errors.Should().BeEmpty();
        }

        [TestCase(null)]
        [TestCase("  ")]
        [TestCase("    ")]
        public void IsDecimalValidator_IsDecimalWhenIsNotNullOrWhitespace_WhenPropertyIsEmptyOrWhiteSpace_IsValid(string propertyValue)
        {
            validator
                .RuleFor(x => x.PropertyValue)
                .IsDecimalWhenIsNotNullOrWhitespace();

            var validationResult = validator.Validate(new PropertyValidator { PropertyValue = propertyValue });

            validationResult.IsValid.Should().BeTrue($"Actual value: '{propertyValue}'");
            validationResult.Errors.Should().BeEmpty();
        }

        public class PropertyValidator
        {
            public string PropertyValue { get; set; }
        }
    }
}
