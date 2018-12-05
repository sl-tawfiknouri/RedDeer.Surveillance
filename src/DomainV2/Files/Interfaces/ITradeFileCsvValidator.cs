using FluentValidation.Results;

namespace DomainV2.Files.Interfaces
{
    public interface ITradeFileCsvValidator
    {
        ValidationResult Validate(TradeFileCsv csv);
    }
}
