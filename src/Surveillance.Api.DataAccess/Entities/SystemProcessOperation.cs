using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
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

        public string Start => OperationStart.ToString(CultureInfo.GetCultureInfo("en-GB")) ?? string.Empty;
        public string End => OperationEnd?.ToString(CultureInfo.GetCultureInfo("en-GB")) ?? string.Empty;
    }
}
