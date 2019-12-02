using FluentValidation.Results;
using NUnit.Framework;
using SharedKernel.Files.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentValidation.Internal;
using System.Linq.Expressions;

namespace SharedKernel.Files.Tests.Orders
{
    public partial class BaseOrderFileValidatorUnitTests
    {
        private BaseOrderFIleValidator validator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            validator = new BaseOrderFIleValidator();
        }

        private static string CreateStringOfLength(int length)
        {
            var sb = new StringBuilder();
            for (int i = 1; i <= length; i++)
            {
                sb.Append(i % 10);
            }
            return sb.ToString();
        }

        private static List<string> GetEnumValidValues<TEnum>()
        {
            var list = new List<string>(Enum.GetNames(typeof(TEnum)));

            foreach (var item in Enum.GetValues(typeof(TEnum)))
            {
                list.Add(((int)item).ToString());
            }

            return list;
        }

        private string FormatMessage(string errorMessage, List<ValidationFailure> logErrors)
           => new StringBuilder($"ErrorMessage: {errorMessage}.")
                .AppendLine("Property Errors:")
                .AppendLine(FormatLogErrorMessage(logErrors))
                .ToString();    

        private string FormatLogErrorMessage(List<ValidationFailure> logErrors)
            => string.Join(Environment.NewLine, logErrors.Select(s => $"{s.ErrorMessage} | {s.ErrorCode} | {s.PropertyName}"));

        private static (string PropertyName, string PropertyDisplayName, string PropertyValue) GetProperty(Expression<Func<OrderFileContract, string>> expression, OrderFileContract orderFileContract)
        {
            return
            (
                PropertyName: expression.GetMember().Name,
                PropertyDisplayName: Extensions.SplitPascalCase(expression.GetMember().Name),
                PropertyValue: expression.Compile()(orderFileContract)
            );
        }

        public class OrderFileValidatorTestData
        {
            public OrderFileContract OrderFileContract { get; set; }

            public string PropertyName { get; set; }

            public string ExpectedMessage { get; set; }

            public string RuleValidator { get; set; }
        }
    }
}
