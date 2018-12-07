namespace DataImport.S3_IO.Interfaces
{
    public interface IFileUploadMessageMapper
    {
        FileUploadMessageDto Map(string json);
    }
}