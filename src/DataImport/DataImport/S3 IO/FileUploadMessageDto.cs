namespace DataImport.S3_IO
{
    /// <summary>
    ///     This is a DTO from a message bus - don't change without chatting to dev ops
    /// </summary>
    public class FileUploadMessageDto
    {
        public string Bucket { get; set; }

        public string FileName { get; set; }

        public long FileSize { get; set; }

        public string VersionId { get; set; }

        public override string ToString()
        {
            return
                $"Bucket: {this.Bucket}, FileName: {this.FileName}, VersionId: {this.VersionId}, FileSize: {this.FileSize}";
        }
    }
}