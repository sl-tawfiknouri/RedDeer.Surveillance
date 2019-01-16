using DataImport.Disk_IO.AllocationFile;
using DataImport.Managers.Interfaces;
using DomainV2.Streams.Interfaces;
using DomainV2.Trading;
using System;

namespace DataImport.Managers
{
    public class OrderAllocationStreamManager : IOrderAllocationStreamManager
    {
        private readonly IOrderAllocationStream<OrderAllocation> _tradeOrderStream;
        private readonly IAllocationFileMonitorFactory _fileMonitorFactory;

        public OrderAllocationStreamManager(
            IOrderAllocationStream<OrderAllocation> tradeOrderStream,
            IAllocationFileMonitorFactory fileMonitorFactory)
        {
            _tradeOrderStream = tradeOrderStream ?? throw new ArgumentNullException(nameof(tradeOrderStream));
            _fileMonitorFactory = fileMonitorFactory ?? throw new ArgumentNullException(nameof(fileMonitorFactory));
        }

        public IUploadAllocationFileMonitor Initialise()
        {
            // hook up the data recorder
            //_tradeOrderStream.Subscribe(_tradeRecorder);

            var fileMonitor = _fileMonitorFactory.Create(_tradeOrderStream);
            fileMonitor.Initiate();

            return fileMonitor;
        }
    }
}
