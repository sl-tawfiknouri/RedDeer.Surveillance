namespace Surveillance.Api.DataAccess.Abstractions.Entities
{
    public interface ISystemProcessOperationUploadFile
    {
        string FilePath { get; set; }

        int FileType { get; set; }

        int Id { get; set; }

        int SystemProcessOperationId { get; set; }
    }
}