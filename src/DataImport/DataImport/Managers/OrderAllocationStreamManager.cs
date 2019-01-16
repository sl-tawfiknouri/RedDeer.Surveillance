using DataImport.Disk_IO.AllocationFile;
using DataImport.Managers.Interfaces;
using DataImport.Recorders.Interfaces;
using DomainV2.Streams;
using DomainV2.Streams.Interfaces;
using DomainV2.Trading;
using System;

namespace DataImport.Managers
{
    public class OrderAllocationStreamManager : IOrderAllocationStreamManager
    {
        private readonly OrderAllocationStream<OrderAllocation> _orderAllocationsStream;
        private readonly IAllocationFileMonitorFactory _fileMonitorFactory;
        private readonly IRedDeerAuroraOrderAllocationRecorder _recorder;

        public OrderAllocationStreamManager(
            OrderAllocationStream<OrderAllocation> tradeOrderStream,
            IAllocationFileMonitorFactory fileMonitorFactory,
            IRedDeerAuroraOrderAllocationRecorder recorder)
        {
            _orderAllocationsStream = tradeOrderStream ?? throw new ArgumentNullException(nameof(tradeOrderStream));
            _fileMonitorFactory = fileMonitorFactory ?? throw new ArgumentNullException(nameof(fileMonitorFactory));
            _recorder = recorder ?? throw new ArgumentNullException(nameof(recorder));
        }

        public IUploadAllocationFileMonitor Initialise()
        {
            // hook up the data recorder
            _orderAllocationsStream.Subscribe(_recorder);

            var fileMonitor = _fileMonitorFactory.Create(_orderAllocationsStream);
            fileMonitor.Initiate();

            return fileMonitor;
        }
    }
}
