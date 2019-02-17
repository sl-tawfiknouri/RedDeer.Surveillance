using FluentValidation.Results;

namespace Domain.Files.Interfaces
{
    public interface ITradeFileCsvValidator
    {
        ValidationResult Validate(TradeFileCsv csv);
    }
}
