using DomainV2.Trading;

namespace DomainV2.Files.Interfaces
{
    public interface ITradeFileCsvToOrderMapper
    {
        TradeFileCsv[] Map(Order order);
        Order Map(TradeFileCsv csv);
        int FailedParseTotal { get; set; }
    }
}