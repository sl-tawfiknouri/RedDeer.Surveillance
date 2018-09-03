using Surveillance.DataLayer.Configuration.Interfaces;

namespace Surveillance.DataLayer.Configuration
{
    public class DatabaseConfiguration : IDatabaseConfiguration
    {
        public string ElasticSearchDomain { get; set; }
        public string ElasticSearchPort { get; set; }
        public string ElasticSearchProtocol { get; set; }
    }
}
