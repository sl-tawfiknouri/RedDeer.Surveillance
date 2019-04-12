using System.ComponentModel.DataAnnotations;
using Surveillance.Api.DataAccess.Abstractions.Entities;

namespace Surveillance.Api.DataAccess.Entities
{
    public class SystemProcessOperationUploadFile : ISystemProcessOperationUploadFile
    {
        public SystemProcessOperationUploadFile()
        {
        }

        [Key]
        public int Id { get; set; }

        public int SystemProcessOperationId { get; set; }
        public int FileType { get; set; }
        public string FilePath { get; set;}
    }
}
