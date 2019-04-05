namespace DataImport.Disk_IO.EtlFile.Interfaces
{
    public interface IUploadEtlFileMonitor
    {
        bool ProcessFile(string path);
        void Initiate();
    }
}