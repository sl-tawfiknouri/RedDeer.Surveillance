namespace Surveillance.DataLayer.Configuration.Interfaces
{
    public interface IDatabaseConfiguration
    {
        string ElasticSearchDomain { get; set; }
        string ElasticSearchPort { get; set; }
        string ElasticSearchProtocol { get; set; }
    }
}