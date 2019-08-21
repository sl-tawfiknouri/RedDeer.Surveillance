namespace Surveillance.Api.DataAccess.Entities
{
    using Surveillance.Api.DataAccess.Abstractions.Entities;

    public class Trader : ITrader
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}