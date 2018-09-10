using System.Collections.Generic;
using Domain.Trades.Orders;

namespace Relay.Disk_IO.Interfaces
{
    public interface IUploadTradeFileProcessor
    {
        UploadTradeFileProcessorResult Process(string path);
        void WriteFailedReadsToDisk(string failedReadsPath, string failedReadFileName, IReadOnlyCollection<TradeOrderFrameCsv> failedReads);
    }
}