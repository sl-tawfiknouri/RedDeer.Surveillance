using DomainV2.Trading;
using System;

namespace DataImport.Recorders.Interfaces
{
    public interface IRedDeerAuroraOrderAllocationRecorder : IObserver<OrderAllocation>
    {
    }
}
