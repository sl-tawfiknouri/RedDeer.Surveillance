namespace RedDeer.Surveillance.Api.Client.Dtos
{
    using System;

    public class ProcessOperationDto
    {
        public int Id { get; set; }

        public DateTime? OperationEnd { get; set; }

        public DateTime OperationStart { get; set; }

        public int OperationState { get; set; }
    }
}