using Domain.Trading;

namespace Domain.Files.Interfaces
{
    public interface ITradeFileCsvToOrderMapper
    {
        TradeFileCsv[] Map(Order order);
        Order Map(TradeFileCsv csv);
        int FailedParseTotal { get; set; }
    }
}