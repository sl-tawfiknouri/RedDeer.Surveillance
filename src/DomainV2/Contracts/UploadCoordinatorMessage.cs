using DomainV2.Files;

namespace DomainV2.Contracts
{
    public class UploadCoordinatorMessage
    {
        public string FileId { get; set; }
        public UploadedFileType Type { get; set; }
    }
}
