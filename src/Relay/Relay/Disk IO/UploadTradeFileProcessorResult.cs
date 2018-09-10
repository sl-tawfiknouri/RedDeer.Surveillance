using System.Collections.Generic;
using Domain.Trades.Orders;

namespace Relay.Disk_IO
{
    public class UploadTradeFileProcessorResult
    {
        public UploadTradeFileProcessorResult(
            IReadOnlyCollection<TradeOrderFrame> successfulReads,
            IReadOnlyCollection<TradeOrderFrameCsv> unsuccessfulReads)
        {
            SuccessfulReads = successfulReads ?? new List<TradeOrderFrame>();
            UnsuccessfulReads = unsuccessfulReads ?? new List<TradeOrderFrameCsv>();
        }

        public IReadOnlyCollection<TradeOrderFrame> SuccessfulReads { get; }
        public IReadOnlyCollection<TradeOrderFrameCsv> UnsuccessfulReads { get; }
    }
}
