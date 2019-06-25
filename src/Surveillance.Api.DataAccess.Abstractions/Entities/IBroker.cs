using System;

namespace Surveillance.Api.DataAccess.Abstractions.Entities
{
    public interface IBroker
    {
        int Id { get; set; }
        string ExternalId { get; set; }
        string Name { get; set; }
        DateTime CreatedOn { get; set; }
    }
}
