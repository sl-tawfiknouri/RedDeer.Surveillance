namespace Surveillance.Api.DataAccess.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Surveillance.Api.DataAccess.Abstractions.Entities;

    public class SystemProcessOperation : ISystemProcessOperation
    {
        [Key]
        public int Id { get; set; }

        public DateTime? OperationEnd { get; set; }

        public DateTime OperationStart { get; set; }

        public int OperationState { get; set; }

        public string SystemProcessId { get; set; }
    }
}