using DomainV2.Trading;

namespace DomainV2.Files.Interfaces
{
    public interface ITradeFileCsvToOrderMapper
    {
        Order Map(TradeFileCsv csv);
        int FailedParseTotal { get; set; }
    }
}