namespace RedDeer.Surveillance.Api.Client.Dtos
{
    using System;

    public class BrokerDto
    {
        public DateTime CreatedOn { get; set; }

        public string ExternalId { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }
    }
}