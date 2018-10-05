using System.Data;

namespace Surveillance.System.DataLayer.Interfaces
{
    public interface IConnectionStringFactory
    {
        string Build();
        IDbConnection BuildConn();
    }
}