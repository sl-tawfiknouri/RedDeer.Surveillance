using DataImport.Disk_IO.AllocationFile;

namespace DataImport.Managers.Interfaces
{
    public interface IOrderAllocationStreamManager
    {
        IUploadAllocationFileMonitor Initialise();
    }
}