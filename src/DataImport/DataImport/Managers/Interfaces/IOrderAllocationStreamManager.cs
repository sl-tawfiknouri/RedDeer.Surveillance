using DataImport.Disk_IO.AllocationFile;
using DataImport.Disk_IO.AllocationFile.Interfaces;

namespace DataImport.Managers.Interfaces
{
    public interface IOrderAllocationStreamManager
    {
        IUploadAllocationFileMonitor Initialise();
    }
}