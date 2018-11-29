using System.Collections.Generic;
using Domain.Equity.Frames;
using Relay.Disk_IO.Interfaces;

namespace Relay.Disk_IO.EquityFile.Interfaces
{
    public interface IUploadEquityFileProcessor : IBaseUploadFileProcessor<SecurityTickCsv, ExchangeFrame>
    {
        void WriteFailedReadsToDisk(string path, string originalFileName, IReadOnlyCollection<SecurityTickCsv> failedReads);
    }
}