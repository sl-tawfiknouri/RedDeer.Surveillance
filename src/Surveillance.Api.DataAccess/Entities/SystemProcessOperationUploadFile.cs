namespace Surveillance.Api.DataAccess.Entities
{
    using System.ComponentModel.DataAnnotations;

    using Surveillance.Api.DataAccess.Abstractions.Entities;

    public class SystemProcessOperationUploadFile : ISystemProcessOperationUploadFile
    {
        public string FilePath { get; set; }

        public int FileType { get; set; }

        [Key]
        public int Id { get; set; }

        public int SystemProcessOperationId { get; set; }
    }
}