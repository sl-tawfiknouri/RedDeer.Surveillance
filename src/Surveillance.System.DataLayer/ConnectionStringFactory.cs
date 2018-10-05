using Surveillance.System.DataLayer.Interfaces;

namespace Surveillance.System.DataLayer
{
    public class ConnectionStringFactory : IConnectionStringFactory
    {
        public string Build()
        {
            return "server=dev-surveillance.cluster-cgedh3fdlw42.eu-west-1.rds.amazonaws.com:3306;uid=reddeer;pwd==6CCkoJb2b+HtKg9;database=dev_surveillance";
        }
    }
}
