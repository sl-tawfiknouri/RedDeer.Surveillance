namespace Surveillance.Api.DataAccess.Entities
{
    using Surveillance.Api.DataAccess.Abstractions.Entities;

    public class Aggregation : IAggregation
    {
        public int Count { get; set; }

        public string Key { get; set; }
    }
}