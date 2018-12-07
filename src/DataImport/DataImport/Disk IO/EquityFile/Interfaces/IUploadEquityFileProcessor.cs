﻿using System.Collections.Generic;
using DataImport.Disk_IO.Interfaces;
using Domain.Equity.Frames;

namespace DataImport.Disk_IO.EquityFile.Interfaces
{
    public interface IUploadEquityFileProcessor : IBaseUploadFileProcessor<SecurityTickCsv, ExchangeFrame>
    {
        void WriteFailedReadsToDisk(string path, string originalFileName, IReadOnlyCollection<SecurityTickCsv> failedReads);
    }
}