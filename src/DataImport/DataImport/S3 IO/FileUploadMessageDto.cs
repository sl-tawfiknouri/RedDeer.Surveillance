namespace DataImport.S3_IO
{
    /// <summary>
    /// This is a DTO from a message bus - don't change without chatting to dev ops
    /// </summary>
    public class FileUploadMessageDto
    {
        public string Bucket { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
    }
}
