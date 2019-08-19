namespace Surveillance.Api.DataAccess.Entities
{
    using System;

    using Surveillance.Api.DataAccess.Abstractions.Entities;

    public class Broker : IBroker
    {
        public DateTime CreatedOn { get; set; }

        public string ExternalId { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }
    }
}