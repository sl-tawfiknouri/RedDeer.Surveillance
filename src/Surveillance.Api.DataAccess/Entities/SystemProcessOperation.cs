using System;
using System.ComponentModel.DataAnnotations;
using Surveillance.Api.DataAccess.Abstractions.Entities;

namespace Surveillance.Api.DataAccess.Entities
{
    public class SystemProcessOperation : ISystemProcessOperation
    {
        public SystemProcessOperation()
        {
        }

        [Key]
        public int Id { get; set; }

        public string SystemProcessId { get; set; }
        public DateTime OperationStart { get; set; }
        public DateTime? OperationEnd { get; set; }
        public int OperationState { get; set; }
    }
}
