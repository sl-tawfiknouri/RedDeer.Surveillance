using Surveillance.Api.DataAccess.Abstractions.Entities;

namespace Surveillance.Api.DataAccess.Entities
{
    public class Trader : ITrader
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
