namespace DataImport.Disk_IO.AllocationFile
{
    public interface IAllocationFileMonitorFactory
    {
        IUploadAllocationFileMonitor Build();
    }
}