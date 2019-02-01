namespace Surveillance.Systems.DataLayer.Processes.Interfaces
{
    public interface ISystemProcessOperationUploadFile
    {
        string FilePath { get; set; }
        int FileType { get; set; }
        int Id { get; set; }
        string SystemProcessId { get; set; }
        int SystemProcessOperationId { get; set; }
    }
}