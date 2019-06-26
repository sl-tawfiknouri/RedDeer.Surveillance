using System;

namespace RedDeer.Surveillance.Api.Client.Dtos
{
    public class ProcessOperationDto
    {
        public int Id { get; set; }
        public DateTime OperationStart { get; set; }
        public DateTime? OperationEnd { get; set; }
        public int OperationState { get; set; }
    }
}
