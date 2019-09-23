namespace Surveillance.Api.DataAccess.Entities
{
    using Surveillance.Api.DataAccess.Abstractions.Entities;

    public class Strategy : IStrategy
    {
        public Strategy(string name)
        {
            this.Name = name ?? string.Empty;
        }

        public string Name { get; }
    }
}