using Surveillance.Api.DataAccess.Abstractions.Entities;

namespace Surveillance.Api.DataAccess.Entities
{
    public class Strategy : IStrategy
    {
        public Strategy(string name)
        {
            Name = name ?? string.Empty;
        }

        public string Name { get; }
    }
}
