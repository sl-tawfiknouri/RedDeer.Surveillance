namespace DataImport.Disk_IO.EtlFile.Interfaces
{
    public interface IUploadEtlFileMonitor
    {
        void Initiate();

        bool ProcessFile(string path);
    }
}