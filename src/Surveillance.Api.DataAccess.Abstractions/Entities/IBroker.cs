namespace Surveillance.Api.DataAccess.Abstractions.Entities
{
    using System;

    public interface IBroker
    {
        DateTime CreatedOn { get; set; }

        string ExternalId { get; set; }

        int Id { get; set; }

        string Name { get; set; }
    }
}