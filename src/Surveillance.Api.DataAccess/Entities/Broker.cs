using Surveillance.Api.DataAccess.Abstractions.Entities;
using System;

namespace Surveillance.Api.DataAccess.Entities
{
    public class Broker : IBroker
    {
        public int Id { get; set; }

        public string ExternalId { get; set; }

        public string Name { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
