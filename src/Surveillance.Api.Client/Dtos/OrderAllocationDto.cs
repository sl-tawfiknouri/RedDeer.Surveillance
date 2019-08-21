namespace RedDeer.Surveillance.Api.Client.Dtos
{
    using System;

    public class OrderAllocationDto
    {
        public bool Autoscheduled { get; set; }

        public string ClientAccountId { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Fund { get; set; }

        public int Id { get; set; }

        public bool Live { get; set; }

        public long OrderFilledVolume { get; set; }

        public string OrderId { get; set; }

        public string Strategy { get; set; }
    }
}