using System;

namespace RedDeer.Surveillance.Api.Client.Dtos
{
    public class BrokerDto
    {
        public int Id { get; set; }

        public string ExternalId { get; set; }

        public string Name { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
